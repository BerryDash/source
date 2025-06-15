using UnityEngine;

public class EncryptedWWWForm
{
    private readonly WWWForm form = new();

    public void AddField(string key, string value)
    {
        form.AddField(key, SensitiveInfo.Encrypt(value, SensitiveInfo.SERVER_SEND_TRANSFER_KEY));
    }

    public WWWForm GetWWWForm() => form;
}
