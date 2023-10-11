using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;


public class LanguageManager : MonoBehaviour {
    public class English {
        public string[] fields;
    }

    public class Swedish {
        public string[] fields;
    }
    
    [Serializable]
    public class Languages {
        public English englishFields;
        public Swedish swedishFields;
    }
    
    public List<TMP_Text> orderedTextToTranslate;
    public TextAsset translationData;
    private IEnumerator Start() {
        var translationText = JsonUtility.FromJson<Languages>(translationData.text);
        yield return new WaitUntil(() => translationText != null);
        
    }
}
