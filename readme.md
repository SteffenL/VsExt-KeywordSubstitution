# Keyword Substitution (extension for Visual Studio)

## About

This extension performs keyword substitution/expansion inside a document's text editor when the document is being saved.

## Build prerequisites
- Visual Studio 2010
- [Visual Studio 2010 SP1 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=21835)

## How to use

At the moment, there's not enough documentation because this extension will mainly be used internally.

Please see the code for more details, especially in the files in the folder `KeywordSubstitution\SubstituteRules\Detail`.

The sample below shows all of the available keywords. Some keywords have aliases, such as `IncrementInteger`  and `FileSaveCounter` being aliases for `IncrementNumber`.

Aliases can currently not be customized without editing code and rebuilding the extension. 

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
