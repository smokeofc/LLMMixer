using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Wpf;

namespace LLMMixer
{
    public partial class MainWindow : Window
    {
        private AppSettings settings = null!;
        private List<WebView2> webViews = null!;
        private List<Border> serviceBorders = null!;
        private List<TextBlock> headers = null!;
        private List<Button> buttons = null!;
        private int? draggedServiceIndex = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCollections();
            LoadSettings();
            InitializeWebViews();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void InitializeCollections()
        {
            webViews = new List<WebView2> { WebView0, WebView1, WebView2, WebView3, WebView4, WebView5, WebView6, WebView7 };
            serviceBorders = new List<Border> { Service0, Service1, Service2, Service3, Service4, Service5, Service6, Service7 };
            headers = new List<TextBlock> { Header0, Header1, Header2, Header3, Header4, Header5, Header6, Header7 };
            buttons = new List<Button> { };  // Now using MenuItems instead
        }

        private void LoadSettings()
        {
            settings = AppSettings.Load();
            
            // Handle migration from old versions
            if (settings.Services.Count == 4 || settings.Services.Count == 6)
            {
                // Old settings detected - migrate to new structure
                var oldServices = new List<ServiceInfo>(settings.Services);
                settings = AppSettings.GetDefaultSettings();
                
                // Preserve the visibility and width settings from old services where names match
                foreach (var oldService in oldServices)
                {
                    var newService = settings.Services.FirstOrDefault(s => s.Name == oldService.Name);
                    if (newService != null)
                    {
                        newService.IsVisible = oldService.IsVisible;
                        newService.Width = oldService.Width;
                    }
                }
            }
            else if (settings.Services.Count < 8)
            {
                // Incomplete settings - fill with defaults
                var defaultSettings = AppSettings.GetDefaultSettings();
                while (settings.Services.Count < 8)
                {
                    if (settings.Services.Count < defaultSettings.Services.Count)
                    {
                        settings.Services.Add(defaultSettings.Services[settings.Services.Count]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            // Sort services by order
            settings.Services = settings.Services.OrderBy(s => s.Order).ToList();
            
            // Update UI based on settings
            for (int i = 0; i < settings.Services.Count && i < 8; i++)
            {
                if (i < headers.Count)
                {
                    var service = settings.Services[i];
                    headers[i].Text = service.Name;
                    
                    // Find the menu item that matches this service name and update its check state
                    var menuItem = FindMenuItem(service.Name);
                    if (menuItem != null)
                    {
                        menuItem.IsChecked = service.IsVisible;
                    }
                }
            }
        }

        private MenuItem? FindMenuItem(string serviceName)
        {
            return serviceName switch
            {
                "ChatGPT" => mnuChatGPT,
                "Claude" => mnuClaude,
                "Gemini" => mnuGemini,
                "Mistral" => mnuMistral,
                "DeepSeek" => mnuDeepSeek,
                "Qwen" => mnuQwen,
                "Grok" => mnuGrok,
                "Kimi" => mnuKimi,
                _ => null
            };
        }

        private async void InitializeWebViews()
        {
            for (int i = 0; i < webViews.Count && i < settings.Services.Count; i++)
            {
                var webView = webViews[i];
                var service = settings.Services[i];
                
                try
                {
                    // Initialize WebView2
                    await webView.EnsureCoreWebView2Async(null);
                    
                    // Set user agent to appear as a normal browser (latest Chrome)
                    webView.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";
                    
                    // Enable features for better web app compatibility
                    webView.CoreWebView2.Settings.IsWebMessageEnabled = true;
                    webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                    webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = true;
                    
                    // Add navigation starting event to add custom headers
                    int serviceIndex = i; // Capture for closure
                    webView.CoreWebView2.NavigationStarting += (sender, args) =>
                    {
                        // Add language and region headers to prefer English content
                        var headers = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.9"),
                            new KeyValuePair<string, string>("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8")
                        };
                        
                        foreach (var header in headers)
                        {
                            args.RequestHeaders.SetHeader(header.Key, header.Value);
                        }
                        
                        // For Qwen specifically, prevent redirect to tongyi.aliyun.com
                        if (settings.Services[serviceIndex].Name == "Qwen" && 
                            args.Uri.Contains("tongyi.aliyun.com") && 
                            !args.Uri.Contains("chat.qwen.ai"))
                        {
                            args.Cancel = true;
                            // Navigate back to the correct URL
                            System.Diagnostics.Debug.WriteLine($"Blocked redirect to: {args.Uri}");
                            webView.CoreWebView2.Navigate("https://chat.qwen.ai/");
                        }
                    };
                    
                    // Navigate to the service URL only if the service is visible
                    if (!string.IsNullOrEmpty(service.Url) && service.IsVisible)
                    {
                        webView.CoreWebView2.Navigate(service.Url);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error initializing {service.Name}: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply saved column widths if any
            ApplyColumnWidths();
            
            // Apply visibility settings after everything is loaded
            for (int i = 0; i < settings.Services.Count && i < 8; i++)
            {
                UpdateServiceVisibility(i, settings.Services[i].IsVisible);
            }
        }

        private void ApplyColumnWidths()
        {
            var columns = new[] { Col0, Col1, Col2, Col3, Col4, Col5, Col6, Col7 };
            for (int i = 0; i < settings.Services.Count && i < columns.Length; i++)
            {
                columns[i].Width = new GridLength(settings.Services[i].Width, GridUnitType.Star);
            }
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            // Save column widths
            var columns = new[] { Col0, Col1, Col2, Col3, Col4, Col5, Col6, Col7 };
            for (int i = 0; i < settings.Services.Count && i < columns.Length; i++)
            {
                if (columns[i].Width.IsStar)
                {
                    settings.Services[i].Width = columns[i].Width.Value;
                }
            }
            
            settings.Save();
        }

        private void ToggleService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string serviceName)
            {
                // Find the service by name (not by index, since order can change)
                var service = settings.Services.FirstOrDefault(s => s.Name == serviceName);
                if (service != null)
                {
                    // Find the current index of this service
                    int index = settings.Services.IndexOf(service);
                    
                    service.IsVisible = !service.IsVisible;
                    
                    menuItem.IsChecked = service.IsVisible;
                    UpdateServiceVisibility(index, service.IsVisible);
                    
                    // If turning ON and WebView hasn't loaded yet, navigate to URL
                    if (service.IsVisible && index >= 0 && index < webViews.Count)
                    {
                        var webView = webViews[index];
                        if (webView.CoreWebView2 != null && 
                            (webView.CoreWebView2.Source == null || webView.CoreWebView2.Source == "about:blank"))
                        {
                            webView.CoreWebView2.Navigate(service.Url);
                        }
                    }
                    
                    SaveSettings();
                }
            }
        }

        private void UpdateServiceVisibility(int index, bool isVisible)
        {
            int columnIndex = index * 2; // Account for splitters
            
            if (columnIndex < MainGrid.ColumnDefinitions.Count)
            {
                var column = MainGrid.ColumnDefinitions[columnIndex];
                column.Width = isVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
                
                // Hide the splitter after this column if the next column is also hidden
                int splitterIndex = columnIndex + 1;
                if (splitterIndex < MainGrid.ColumnDefinitions.Count)
                {
                    bool shouldShowSplitter = isVisible && IsAnyServiceVisibleAfter(index);
                    MainGrid.ColumnDefinitions[splitterIndex].Width = shouldShowSplitter ? new GridLength(5) : new GridLength(0);
                }
            }
        }

        private bool IsAnyServiceVisibleAfter(int index)
        {
            for (int i = index + 1; i < settings.Services.Count; i++)
            {
                if (settings.Services[i].IsVisible)
                    return true;
            }
            return false;
        }

        private void ResetLayout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("This will reset all layout settings to default. Continue?", 
                "Reset Layout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                settings = AppSettings.GetDefaultSettings();
                LoadSettings();
                ApplyColumnWidths();
                
                // Refresh all WebViews
                for (int i = 0; i < webViews.Count && i < settings.Services.Count; i++)
                {
                    var webView = webViews[i];
                    var service = settings.Services[i];
                    
                    if (webView.CoreWebView2 != null && !string.IsNullOrEmpty(service.Url))
                    {
                        webView.CoreWebView2.Navigate(service.Url);
                    }
                }
                
                SaveSettings();
            }
        }

        private void RefreshAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var webView in webViews)
            {
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Reload();
                }
            }
        }

        private void StartNewChatAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < webViews.Count && i < settings.Services.Count; i++)
            {
                var webView = webViews[i];
                var service = settings.Services[i];
                
                if (webView.CoreWebView2 != null && !string.IsNullOrEmpty(service.Url))
                {
                    webView.CoreWebView2.Navigate(service.Url);
                }
            }
        }

        private void RefreshService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tagStr)
            {
                int index = int.Parse(tagStr);
                
                if (index >= 0 && index < webViews.Count)
                {
                    var webView = webViews[index];
                    if (webView.CoreWebView2 != null)
                    {
                        webView.CoreWebView2.Reload();
                    }
                }
            }
        }

        // Drag and Drop functionality for reordering
        private void ServiceHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is TextBlock textBlock)
            {
                // Navigate up the visual tree to find the service Border
                // Structure: TextBlock -> Grid -> Border (header) -> Grid -> Border (Service0-5)
                DependencyObject current = textBlock;
                Border? serviceBorder = null;
                
                // Go up the tree to find the main service Border
                while (current != null && serviceBorder == null)
                {
                    current = VisualTreeHelper.GetParent(current);
                    if (current is Border border && border.Name != null && border.Name.StartsWith("Service"))
                    {
                        serviceBorder = border;
                    }
                }
                
                if (serviceBorder != null)
                {
                    // Find which service index this is
                    for (int i = 0; i < serviceBorders.Count; i++)
                    {
                        if (serviceBorders[i] == serviceBorder)
                        {
                            draggedServiceIndex = i;
                            DragDrop.DoDragDrop(textBlock, i, DragDropEffects.Move);
                            draggedServiceIndex = null;
                            break;
                        }
                    }
                }
            }
        }

        private void Service_DragOver(object sender, DragEventArgs e)
        {
            if (draggedServiceIndex.HasValue)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Service_Drop(object sender, DragEventArgs e)
        {
            if (!draggedServiceIndex.HasValue || sender is not Border targetBorder)
                return;

            int sourceIndex = draggedServiceIndex.Value;
            
            // Find target index
            int targetIndex = -1;
            for (int i = 0; i < serviceBorders.Count; i++)
            {
                if (serviceBorders[i] == targetBorder)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex >= 0 && targetIndex != sourceIndex)
            {
                SwapServices(sourceIndex, targetIndex);
            }
        }

        private void SwapServices(int index1, int index2)
        {
            // Swap in settings
            var temp = settings.Services[index1];
            settings.Services[index1] = settings.Services[index2];
            settings.Services[index2] = temp;
            
            // Update orders
            settings.Services[index1].Order = index1;
            settings.Services[index2].Order = index2;
            
            // Swap header texts
            string tempText = headers[index1].Text;
            headers[index1].Text = headers[index2].Text;
            headers[index2].Text = tempText;
            
            // Update menu item check states for both swapped services
            var menuItem1 = FindMenuItem(settings.Services[index1].Name);
            var menuItem2 = FindMenuItem(settings.Services[index2].Name);
            
            if (menuItem1 != null)
            {
                menuItem1.IsChecked = settings.Services[index1].IsVisible;
            }
            if (menuItem2 != null)
            {
                menuItem2.IsChecked = settings.Services[index2].IsVisible;
            }
            
            // Swap WebView sources
            if (webViews[index1].CoreWebView2 != null && webViews[index2].CoreWebView2 != null)
            {
                string url1 = webViews[index1].CoreWebView2.Source;
                string url2 = webViews[index2].CoreWebView2.Source;
                
                webViews[index1].CoreWebView2.Navigate(settings.Services[index1].Url);
                webViews[index2].CoreWebView2.Navigate(settings.Services[index2].Url);
            }
            
            SaveSettings();
        }
    }
}
