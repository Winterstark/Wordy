Const ForReading = 1
Const ForWriting = 2

strTxtFile = "C:\dev\projex\Wordy\Wordy\bin\Debug\newwords.txt"

Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile(strTxtFile , ForReading)

if objFile.AtEndOfStream <> True Then
	strContents = objFile.ReadAll
Else
	strContents = ""
End If

objFile.Close

For Each strArg in Wscript.Arguments
	If strContents = "" Then
		strContents = strArg
	Else
		strContents = strContents & vbCrLf & strArg
	End If
Next

Set objFile = objFSO.OpenTextFile(strTxtFile , ForWriting)
objFile.Write strContents

objFile.Close