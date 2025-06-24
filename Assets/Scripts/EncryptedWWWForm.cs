using UnityEngine;

public class EncryptedWWWForm
{
    private readonly WWWForm form = new();

    public void AddField(string key, string value)
    {
        form.AddField(SensitiveInfo.Encrypt(key, SensitiveInfo.SERVER_SEND_TRANSFER_KEY), SensitiveInfo.Encrypt(value, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
    }

    public WWWForm GetWWWForm() => form;
}
