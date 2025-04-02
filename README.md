# Lua Script Editor for Android Platforms

![Lua Logo](https://www.lua.org/images/logos/lua-logo.gif)

A lightweight Lua script editor and runner, built with .NET MAUI in C#, but only intended for Android platforms.

## Features

- Syntax highlighting for Lua code (keywords, strings, numbers, comments)
- Code execution with output display
- Clean, intuitive interface
- Error handling for script execution

## Getting Started

### Prerequisites
- Android device/emulator with API level 24+
- .NET MAUI development environment

### Installation
1. Clone this repository, using the `git clone` command
2. Open in Visual Studio 2022
3. Build and deploy to your Android device/emulator

## Usage

1. Type or paste your Lua code in the editor
2. Tap "Run" to execute the script
3. View output in the results screen
4. Use "Back" to return to editing

### Important Notes
⚠️ **Warning about infinite loops**:
- Scripts with infinite loops (without `break` statements) will crash the app
- The execution environment has no timeout mechanism
- Example dangerous code:
  ```lua
  while true do end  -- This will crash the app
  ```

## Syntax Highlighting
The editor supports highlighting for:
- Keywords (`if`, `then`, `end`, etc.)
- Built-in functions (`print`, `table`, etc.)
- Strings and numbers
- Comments (single-line and multi-line)

## Known Limitations
- No `input()` function support
- Limited error recovery for malformed scripts
- No file save/load functionality
- No auto-completion

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](https://choosealicense.com/licenses/mit/)

**Have a nice day scripting in Lua!** 🚀
