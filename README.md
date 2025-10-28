# LLMMixer - Multi-Service AI Chat Client

A Windows desktop application that allows you to use multiple AI chat services in a single window.

## ⚠️ Important Notice

**This application was vibe-coded utilizing Claude Sonnet 4.5 through GitHub Copilot with minor manual adjustments.**

As such, operation is not guaranteed and the app has **not undergone a security audit**. Use at your own risk. This is an experimental tool for personal productivity and should not be used in security-sensitive or production environments.

## Features

- **Multiple AI Services in One Window**: Access ChatGPT, Claude, Gemini, Mistral Le Chat, DeepSeek, Qwen, Grok, and Kimi side-by-side
- **Show/Hide Services**: Toggle visibility of individual services using the dropdown menu
- **Drag-and-Drop Reordering**: Rearrange service panels by dragging their headers
- **Individual Refresh Buttons**: Refresh each service independently with the 🔄 icon
- **Persistent Settings**: Your layout preferences and login sessions are automatically saved
- **Resizable Panels**: Adjust the width of each panel using grid splitters
- **Modern UI**: Clean, professional interface with smooth interactions

## Supported Services

1. **ChatGPT** - https://chat.openai.com
2. **Claude** - https://claude.ai
3. **DeepSeek** - https://chat.deepseek.com
4. **Gemini** - https://gemini.google.com
5. **Grok** - https://grok.com
6. **Kimi** - https://www.kimi.com/en/
7. **Mistral Le Chat** - https://chat.mistral.ai
8. **Qwen** - https://chat.qwen.ai

## Requirements

- Windows 10 version 1809 or later / Windows 11
- .NET 8.0 Runtime or later
- WebView2 Runtime (usually pre-installed on Windows 11)

## Installation

### Option 1: Build from Source

1. **Install Prerequisites**:
   - [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) with ".NET desktop development" workload
   - [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later

2. **Clone or Download the Repository**:
   ```bash
   git clone <repository-url>
   cd LLMMixer
   ```

3. **Build the Project**:
   ```bash
   cd LLMMixer
   dotnet restore
   dotnet build
   ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

### Option 2: Using Visual Studio

1. Open `LLMMixer.sln` in Visual Studio 2022
2. Press `F5` to build and run the application

## Usage

### Basic Usage

1. **Launch the Application**: Start LLMMixer - all eight services will load automatically
2. **Login to Services**: Click on each panel to log into the respective AI service
3. **Toggle Services**: Click "Services ▼" dropdown menu and use checkboxes to show/hide individual services
4. **Refresh Individual Service**: Click the 🔄 button next to any service name to refresh just that service
5. **Reorder Services**: Click and drag a service name (not the refresh button) to reorder panels
6. **Resize Panels**: Drag the splitters between panels to adjust their width
7. **Refresh All Services**: Click "Refresh All" to reload all active services at once
8. **Start New Chat on All**: Click "Start New Chat on All" to navigate all services back to their landing pages for fresh conversations
9. **Reset Layout**: Click "Reset Layout" to restore default settings

**Note**: To drag and reorder services, click and hold on the service name text (e.g., "ChatGPT", "Claude"), not the refresh button. The cursor will change to a hand when hovering over draggable areas.

### Keyboard Shortcuts

- **F5**: Refresh all services (when focused on main window)
- **Alt+F4**: Close application

### Settings Storage

Settings are automatically saved to:
```
%APPDATA%\LLMMixer\settings.json
```

This includes:
- Service visibility states
- Panel widths
- Service order

**Migration from older versions**: If you're upgrading from a version with 4 or 6 services, your settings will be automatically migrated to support 8 services while preserving your visibility preferences. If you experience any issues after upgrading, you can delete the settings file to start fresh.

## Troubleshooting

### Service Mapping Issues

If buttons are opening the wrong services after an upgrade:

1. Close the application
2. Navigate to `%APPDATA%\LLMMixer\`
3. Delete `settings.json`
4. Restart the application

This will create fresh settings with the correct service mapping.

### WebView2 Not Found

If you get an error about WebView2 Runtime not being installed:

1. Download and install [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/)
2. Restart the application

### Services Not Loading

- Check your internet connection
- Try clicking "Refresh All" button
- Some services may require VPN in certain regions
- Clear browser cache by deleting the WebView2 user data folder:
  ```
  %LOCALAPPDATA%\LLMMixer\
  ```

### Login Issues

- You need to log into each service individually within the app
- Login sessions are maintained by WebView2 (similar to Microsoft Edge browser)
- If you're logged out unexpectedly, try logging in again

## Development

### Project Structure

```
LLMMixer/
├── LLMMixer/
│   ├── App.xaml              # Application resources and styles
│   ├── App.xaml.cs           # Application entry point
│   ├── MainWindow.xaml       # Main window UI layout
│   ├── MainWindow.xaml.cs    # Main window logic
│   ├── ServiceConfig.cs      # Configuration and settings management
│   └── LLMMixer.csproj       # Project file
├── PROJECT_DETAILS.md        # Detailed project documentation
└── README.md                 # This file
```

### Adding New Services

To add a new AI chat service:

1. Open `ServiceConfig.cs`
2. Add a new `ServiceInfo` entry to the `GetDefaultSettings()` method
3. Update `MainWindow.xaml` to add a new WebView2 control and button
4. Update `MainWindow.xaml.cs` to initialize the new service

### Technologies Used

- **WPF (Windows Presentation Foundation)**: UI framework
- **WebView2**: Embedded Microsoft Edge for web content
- **Newtonsoft.Json**: Settings serialization
- **.NET 8.0**: Runtime framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Credits

Built with:
- [Microsoft WebView2](https://developer.microsoft.com/microsoft-edge/webview2/)
- [.NET](https://dotnet.microsoft.com/)

## Support

For issues or questions:
- Create an issue on the repository
- Check existing issues for solutions

---

**Note**: This application is a third-party client and is not affiliated with or endorsed by OpenAI, Anthropic (Claude), Google (Gemini), Mistral AI, DeepSeek, Alibaba (Qwen), xAI (Grok), or Moonshot AI (Kimi).
