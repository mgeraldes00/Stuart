using System.Collections;

public interface IController
{
    void InitializeScene();
    void ActivateBounds();
    void SetDialogue(
        string [] playerLines, string[] followerLines, string[] otherLines);
    void SetThought(string thought);
    void BeginEvent(int i);
    void CollectCoin();
    IEnumerator Dialogue(
        int[] dialogue, float timeToStart, int[] extraParams = null);
}
