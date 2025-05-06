# QR Code Generator

## 🌐 Live Website
[https://anggabard.github.io/QR/](https://anggabard.github.io/QR/)

## 🛠️ API Endpoint
[https://qrgenerator-anggabard.azurewebsites.net/api/QRG](https://qrgenerator-anggabard.azurewebsites.net/api/QRG)

## 📦 API Request Format

Send a POST request to the API endpoint with the following JSON body:

```json
{
  "Message": "https://www.qrcode.com/",
  "AsBinary": true
}

🔹 Message
Type: string

Required: ✅

Description: The message that will be encoded into the QR code.

🔹 AsBinary
Type: boolean

Required: ❌ (optional)

Default: false

Description:

If false (default), the API returns the QR code as a string-based SVG representation.

