using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StarshipDeliveryMod;
public class CustomPositioningToolEvents : MonoBehaviour
{
    public Action? onFieldUpdate;
    public Action? onCopyToClipboard;
    public Action? onOverwriteFile;
    public Action? onCancel;
    public Action? onConfirm;

    public void OnValueAdded(InputField _field)
    {
        float result;
        if (float.TryParse(_field.text, out result))
        {
            float newValue = result + 0.5f;
            _field.text = newValue.ToString();
            onFieldUpdate?.Invoke();
        }
    }

    public void OnValueSubtracted(InputField _field)
    {
        float result;
        if (float.TryParse(_field.text, out result))
        {
            float newValue = result - 0.5f;
            _field.text = newValue.ToString();
            onFieldUpdate?.Invoke();
        }
    }
    public void OnValidateField()
    {
        Debug.Log("ValidateField");
        onFieldUpdate?.Invoke();
    }

    public void OnCopyToClipboard()
    {
        onCopyToClipboard?.Invoke();
    }

    public void OnOverwriteFile()
    {
        onOverwriteFile?.Invoke();
    }
    public void OnCancel()
    {
        onCancel?.Invoke();
    }
    public void OnConfirm()
    {
        onConfirm?.Invoke();
    }
    public void ClosePannel(GameObject _panelToClose)
    {
        _panelToClose.SetActive(false);
    }
}
