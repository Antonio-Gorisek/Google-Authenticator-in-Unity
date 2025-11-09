# 🔐 Unity Two-Factor Authentication (TOTP)

Unity project implementing 2FA using TOTP compatible with Google Authenticator.        
Generates a secret key, displays a QR code for scanning and verifies code.

![WhatsApp Slika 2025-11-09 u 17 37 30_b6a05e5f](https://github.com/user-attachments/assets/340746d4-2e03-47db-8373-29efbc390bce)

## Notes

- You must keep the secret key on the server, never on the local!
- Device time must be accurate for TOTP.
- Each user should have a unique secret key.

## Flow

Open scene > Generate QR > Scan in Authenticator > Enter code > Verify.
