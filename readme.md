# Keyword Substitution (extension for Visual Studio)

## About

This extension performs keyword substitution/replacement/expansion inside a document's text editor when the document is being saved.

## Building and debugging

### Build prerequisites

- Visual Studio 2010
- [Visual Studio 2010 SP1 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=21835)

### Debugging

To debug extensions, open the properties of the project (named "KeywordSubstitution"), go to the Debug tab and fill in the following:
- Start external program: (path to `devenv.exe`, usually `C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe`)
- Command line arguments: `/rootSuffix Exp`

## Usage requirements

- Visual Studio 2010, 2012 or 2013 (Express editions are not supported)

## How to use

Keyword substitution will be done on the open text documents (code or not) that you save. The changes will be done inside a document's text editor before it's saved so that you may see the changes as well as undo them within the editor.

**Keyword format:** `$KeywordName: Value$` (please see samples below)

### Available keywords

#### Default keywords

| Keyword           | Description                                                             |
| ----------------- | ----------------------------------------------------------------------- |
| ProjectDir        | The project's directory path.                                           |
| FilePath          | The file's path.                                                        |
| FileRelativePath  | The file's path relative to the project's directory path.               |
| FileRelativeDir   | The file's directory path relative to the project's directory path.     |
| FileName          | the full name of the file.                                              |
| IncrementNumber   | A simple counter that increments the value.                             |
| UserName          | The logged-on Windows user's name.                                      |
| DateTime          | the current date and time with offset, in a universal format.           |
| MachineName       | The name of the machine (computer name).                                |
| Guid              | A new GUID.                                                             |

#### Internal aliases

Keywords can have alternative names by default.

| Alias             | Keyword               |
| ----------------- | --------------------- |
| IncrementInteger  | IncrementNumber       |

#### Custom aliases (currently hardcoded)

Can currently not be customized without editing code and rebuilding the extension.

| Alias             | Keyword               |
| ----------------- | --------------------- |
| FilePathRootHint  | ProjectDir            |
| FileSaveCounter   | IncrementNumber       |
| FileSaveUser      | UserName              |
| FileSaveDateTime  | DateTime              |
| FileSaveMachine   | MachineName           |
| FileSaveGuid      | Guid                  |

### Examples (used in your document)

	$ProjectDir: $
	$FilePathRootHint: $
	
	$FilePath: $
	$FileRelativePath: $
	$FileRelativeDir: $
	$FileName: $
	
	$IncrementNumber: $
	$IncrementInteger: $
	$FileSaveCounter: $
	
	$UserName: $
	$FileSaveUser: $
	
	$DateTime: $
	$FileSaveDateTime: $
	
	$MachineName: $
	$FileSaveMachine: $
	
	$Guid: $
	$FileSaveGuid: $
