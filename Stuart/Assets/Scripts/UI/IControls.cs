using System.Collections;

public interface IControls
{
    IEnumerator RevealControl(bool onlyPart);
    IEnumerator HideControl(bool onlyPart);
}
