# AksharaShift (അക്ഷരഷിഫ്റ്റ്)

**AksharaShift** is a lightweight, offline Windows system tray utility for checking and converting **Malayalam Unicode text** into legacy ML (TTKarthika) and FML (Revathi) font formats.

It runs silently in the background and offers a quick popup for text conversion, designed for users who work with legacy Malayalam desktop publishing software.

## Features

- **System Tray Integration**: Minimized to tray; click to open, right-click to exit.
- **Offline Conversion**: No internet required. All logic is embedded.
- **Support for Two Legacy Formats**:
  - **ML (Karthika)**: Common legacy font encoding.
  - **FML (Revathi)**: Phonetic legacy font encoding.
- **Automatic Clipboard Handling**: Converted text is automatically copied to the clipboard.
- **Normalize & Fix**: Automatically fixes common Unicode typing errors (e.g., split vowels, chillu letters).

## Usage

1. **Run the Application**: Launch `AksharaShift.exe`. It will appear in your system tray (bottom-right notification area).
2. **Open Converter**: Click the tray icon.
3. **Convert**:
   - Paste Unicode Malayalam text into the input box.
   - Click **Convert To ML** (for Karthika) or **Convert To FML** (for Revathi).
4. **Paste**: The result is automatically copied to your clipboard. Paste it into your legacy application (Photoshop, PageMaker, etc.).

## Installation

1. Download the latest release.
2. Extract the files.
3. Run `AksharaShift.exe`.

## Technical Details

- **Framework**: .NET 8.0 (WPF)
- **License**: MIT
