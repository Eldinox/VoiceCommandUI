using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;
using DG.Tweening;

public class VoiceInput : MonoBehaviour
{
    private Dictionary<string, System.Action> keywordActions = new Dictionary<string, System.Action>();
    private KeywordRecognizer keywordRecognizer;

    [SerializeField]private RectTransform menuPanel;

    void Start()
    {
        ///DICTIONARY///
        keywordActions.Add("menü öffnen", Open);
        keywordActions.Add("menü schließen", Close);
        keywordActions.Add("test", Test);
        ///DICTIONARY///

        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
        keywordRecognizer.Start();
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        keywordActions[args.text].Invoke();
    }

    ///Ausführbare Methoden///
    private void Open()
    {
        Debug.Log("OPEN");
        menuPanel.DOAnchorPos(new Vector2(0, 0), .3f);
    }
    private void Close()
    {
        Debug.Log("CLOSE");
        menuPanel.DOAnchorPos(new Vector2(600, 0), .3f);
    }
    private void Test()
    {
        Debug.Log("Test");
    }
}
