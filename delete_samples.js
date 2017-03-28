/*
 * Script to delete samples directory
 *
 */

var objWshShell = new ActiveXObject("WScript.Shell");
var objFileSys  = new ActiveXObject("Scripting.FileSystemObject");

var pathToAppData = objWshShell.SpecialFolders("Appdata");

var pathToSample = objFileSys.BuildPath(pathToAppData, "asc\\");

var existsSample = objFileSys.FolderExists(pathToSample);

if (existsSample) {

	var objSampleFolder = objFileSys.GetFolder(pathToSample);

	objSampleFolder.Delete(true);

}

objSampleFolder = null;
objFileSys = null;
objWshShell = null;
