using UnityEngine.UI;
using UnityEngine;
using QRCodeShareMain;
using OtpNet;
using TMPro;

/// <summary>
/// TwoFactorAuth implements two-factor authentication (2FA) using Google Authenticator.
/// - Generates one-time codes based on secret key and current time.
/// - The QR code allows the user to easily add an account to the Authenticator application.
/// </summary>
public class TwoFactorAuth : MonoBehaviour {
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private RawImage qrImage;
    [SerializeField] private Button btnConfirm;

    private string base32Secret; // User secret key in Base32 format (MUST BE LOCATED ON THE SERVER NOT LOCAL !!!)
    private string otpauthUrl; // URL for Google Authenticator

    private void Awake() => btnConfirm.onClick.AddListener(OnVerifyCode);

    void Start() => GenerateSecretAndQr(Application.productName, "Antonio");

    /// <summary>
    /// Generates secret key, otpauth URL and QR code for Google Authenticator.
    /// </summary>
    /// <param name="issuer">Publisher or App Name</param>
    /// <param name="account">User account (email or username)</param>
    public void GenerateSecretAndQr(string issuer, string account) {
        // A unique secret key (secretKey) should be generated on the server when a user registers.
        // The secret key is encoded in Base32 and embedded in the otpauth URL which is converted to a QR code.
        // "secretKey" should never be stored locally!
        // The secret should not be changed after it has been assigned to the user.
        // This is for testing and research purposes only.
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        base32Secret = Base32Encoding.ToString(secretKey);

        // 2) Creation of the otpauth URL according to the Google Authenticator standard
        // Format: otpauth://totp/Issuer:Account?secret=BASE32SECRET&issuer=Issuer&digits=6&period=30
        otpauthUrl = $"otpauth://totp/{issuer}:{account}?secret={base32Secret}&issuer={issuer}&digits=6&period=30";

        Debug.Log("Secret key (save in database): " + base32Secret);

        // 3) Generating a QR code using a QR asset
        QRImageProperties properties = new QRImageProperties(500, 500, 50); // width, height, margin
        Texture2D qrTexture = QRCodeShare.CreateQRCodeImage(otpauthUrl, properties);

        // 4) Showing QR code in RawImage
        qrImage.texture = qrTexture;
        qrImage.GetComponent<RectTransform>().sizeDelta = new Vector2(qrTexture.width, qrTexture.height);
    }

    public void OnVerifyCode() {
        string userInput = codeInput.text.Trim();

        if (string.IsNullOrEmpty(userInput)) {
            resultText.text = "Field is empty, enter your code.";
            return;
        }

        // secret key "base32Secret" must be taken from server!!!
        bool valid = VerifyCode(base32Secret, userInput);
        resultText.text = valid ? "The code is correct!" : "Wrong code!";
        Invoke(nameof(ClearTesultText), 2);
    }

    private void ClearTesultText() => resultText.text = string.Empty;

    /// <summary>
    /// It verifies the user's input using the TOTP algorithm.
    /// </summary>
    /// <param name="base32Secret">Secret key in Base32 format</param>
    /// <param name="userInput">The code entered by the user</param>
    /// <returns>True if the code is correct, otherwise false</returns>
    public static bool VerifyCode(string base32Secret, string userInput) {
        var secretKey = Base32Encoding.ToBytes(base32Secret);
        var totp = new Totp(secretKey);

        // VerificationWindow(1,1) allows verification for both the previous and next interval (30s)
        return totp.VerifyTotp(userInput, out _, new VerificationWindow(1, 1));
    }
}
