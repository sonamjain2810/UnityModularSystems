using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Obvious.Soap;
using TMPro;

/// <summary>
/// LevelLoader: loads LevelDataSO into the scene UI (slots + alphabet option pool).
/// - For FillInBlank mode it pre-fills non-blank slots with visual-only Alphabets (non-draggable)
///   and shows the missing letter in the pool (while preventing duplicates).
/// - For Spelling it simply shows all options (pool) and slots empty so player fills all.
/// </summary>
public class LevelLoader : MonoBehaviour
{
    [Header("Scene References (assign exact scene objects in inspector)")]
    [SerializeField] private List<AlphabetSlot> alphabetSlots; // left→right slots in order (Scene)
    [SerializeField] private List<Alphabet> alphabetObjects;   // draggable alphabet objects (pool)

    [Tooltip("Optional: a visual-only prefab (with same visuals but no drag logic). If null, will clone from pool.")]
    [SerializeField] private Alphabet visualAlphabetPrefab;

    [SerializeField] private Image wordImageDisplay;

    [Header("Level Data")]
    [SerializeField] private LevelDatabaseSO levelDatabase;

    [Header("Mode System (SOAP)")]
    [SerializeField] private ScriptableEventScriptableEnumLevelType onModeSelected;
    [SerializeField] private ScriptableEnumLevelType currentMode; // updated by event
    [SerializeField] private ScriptableEventNoParam onNextLevel;  // raised by next button

    [SerializeField] private ScriptableEventAlphabetLetter onCorrectLetterPlaced;

    private int currentLevelIndex = 0;
    private LevelDataSO currentLevelData;
    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
            Debug.LogWarning("⚠️ LevelLoader: No LevelManager found in scene.");
    }

    private void OnEnable()
    {
        if (onModeSelected != null)
            onModeSelected.OnRaised += HandleModeChange;

        if (onNextLevel != null)
            onNextLevel.OnRaised += HandleNextLevel;
    }

    private void OnDisable()
    {
        if (onModeSelected != null)
            onModeSelected.OnRaised -= HandleModeChange;

        if (onNextLevel != null)
            onNextLevel.OnRaised -= HandleNextLevel;
    }

    private void Start()
    {
        if (currentMode != null)
            LoadLevelFromMode(currentMode, currentLevelIndex);
    }

    private void HandleModeChange(ScriptableEnumLevelType newMode)
    {
        currentMode = newMode;
        currentLevelIndex = 0;
        LoadLevelFromMode(currentMode, currentLevelIndex);
    }

    private void HandleNextLevel()
    {
        Debug.Log("➡️ Next Level requested via SOAP event!");
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        var levels = GetCurrentModeLevels();
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("❌ LevelLoader: No levels found for current mode!");
            return;
        }

        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0; // loop back when finished

        LoadLevelFromMode(currentMode, currentLevelIndex);
    }

    private void LoadLevelFromMode(ScriptableEnumLevelType mode, int index)
    {
        var levels = GetCurrentModeLevels();
        if (levels == null || levels.Length == 0) return;

        index = Mathf.Clamp(index, 0, levels.Length - 1);
        LoadLevel(levels[index]);
    }

    private LevelDataSO[] GetCurrentModeLevels()
    {
        if (levelDatabase == null) return null;
        if (currentMode == null) return null;

        if (currentMode.name == "Spelling") return levelDatabase.spellingLevels;
        if (currentMode.name == "FillInBlank") return levelDatabase.fillInBlankLevels;
        if (currentMode.name == "BlankSpelling") return levelDatabase.blankSpellingLevels;
        return null;
    }

    public void LoadLevel(LevelDataSO data)
    {
        if (data == null)
        {
            Debug.LogError("❌ LevelLoader.LoadLevel: data is null!");
            return;
        }

        currentLevelData = data;
        Debug.Log($"📖 Loading Level: {data.word}");

        // Reset visuals & states
        ResetScene();

        // Set image
        if (wordImageDisplay != null && data.wordImage != null)
            wordImageDisplay.sprite = data.wordImage;

        // Setup pool alphabets: assign ScriptableEnumAlphabet and visual values
        for (int i = 0; i < alphabetObjects.Count; i++)
        {
            var alphaObj = alphabetObjects[i];
            if (i < data.letters.Length)
            {
                alphaObj.alpha = data.letters[i];

                var tmp = alphaObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmp != null) tmp.text = data.letters[i].character;

                var img = alphaObj.GetComponentInChildren<Image>();
                if (img != null) img.color = data.letters[i].color;

                alphaObj.ResetToOriginal();
                alphaObj.gameObject.SetActive(true);
            }
            else
            {
                alphaObj.gameObject.SetActive(false);
            }
        }

        // Setup slots
        for (int i = 0; i < alphabetSlots.Count; i++)
        {
            if (i < data.slots.Length)
            {
                alphabetSlots[i].UpdateSlot(data.slots[i]);
                alphabetSlots[i].gameObject.SetActive(true);
            }
            else
            {
                alphabetSlots[i].gameObject.SetActive(false);
            }
        }

        // Apply mode rules
        ApplyModeBehavior(data);

        // Hand level data to LevelManager
        if (levelManager != null)
            levelManager.InitializeLevel(data);
    }

    private void ApplyModeBehavior(LevelDataSO data)
    {
        if (currentMode == null)
        {
            ShowAllLetters();
            return;
        }

        switch (currentMode.name)
        {
            case "Spelling":
                ShowAllLetters();
                break;

            case "FillInBlank":
                ApplyFillInBlankBehavior(data);
                break;

            case "BlankSpelling":
                HideAllLetters();
                break;

            default:
                ShowAllLetters();
                break;
        }
    }

    /// <summary>
    /// Fill-in-Blank mode:
    /// -> Prefill non-blank slots with non-draggable visual clones.
    /// -> Keep the missing letter active as the draggable option.
    /// -> Remove pool entries for prefilled letters so there are no duplicates.
    /// </summary>
    private void ApplyFillInBlankBehavior(LevelDataSO data)
    {
        if (data == null || data.slots == null || data.letters == null)
            return;

        if (levelManager != null)
            levelManager.InitializeLevel(data);

        Debug.Log($"🧩 FillInBlank Mode → BlankIndex = {data.blankIndex}");

        HashSet<ScriptableEnumAlphabet> usedInSlots = new();

        // 1️⃣ Reset all pool alphabets
        foreach (var alpha in alphabetObjects)
        {
            if (alpha == null) continue;

            alpha.ResetToOriginal();
            alpha.gameObject.SetActive(true);
            alpha.enabled = true;

            var cg = alpha.GetComponent<CanvasGroup>() ?? alpha.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        // 2️⃣ Setup slots & prefill visuals (non-draggable)

        for (int i = 0; i < alphabetObjects.Count; i++)
        {
            Alphabet temp = alphabetObjects[i].gameObject.GetComponent<Alphabet>();
            string str = temp.alpha.character.ToString();
            Debug.Log($"📢 Rikhil str: {str}");
            if (str.Equals(data.slots[data.blankIndex]))
            {
                Debug.Log($"📢 Rikhil str: {str}");
            }
            else
            {
                
                //alphabetObjects[i].transform.SetParent(alphabetSlots[i].transform);


                // Debug.Log($"📢 Raised OnCorrectLetterPlaced for prefilled letter: {expected.character}");
            }
        }

        /*for (int i = 0; i < alphabetSlots.Count; i++)
            {   
                if (i >= data.slots.Length)
                {
                    alphabetSlots[i].gameObject.SetActive(false);
                    continue;
                }

                var slot = alphabetSlots[i];
                slot.gameObject.SetActive(true);

                var tmp = slot.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmp != null) tmp.text = "";

                // destroy old children with Alphabet component
                foreach (Transform child in slot.transform)
                    if (child.GetComponent<Alphabet>() != null)
                        Destroy(child.gameObject);

                // Blank slot
                if (data.hasBlank && i == data.blankIndex)
                {
                    if (tmp != null) tmp.text = "_";
                    continue;
                }

                // Prefill other slots visually
                var expected = data.slots[i].expectedLetter;
                if (expected == null) continue;

                Alphabet source;
                if (visualAlphabetPrefab != null)
                {
                    source = visualAlphabetPrefab;
                }
                else
                {
                    source = alphabetObjects.Find(a => a.alpha == expected);
                    source.gameObject.transform.parent = alphabetSlots[i].gameObject.transform;
                }

                if (source == null)
                {
                    if (tmp != null) tmp.text = expected.character;
                    continue;
                }


                Transform optionParent = alphabetObjects[0].transform.parent;
                Alphabet filled = Instantiate(source, optionParent);
                filled.name = expected.character + "_OptionFill";
                filled.gameObject.SetActive(true);
                filled.enabled = true;

                // Align visually under slot
                var slotRT = slot.GetComponent<RectTransform>();
                var filledRT = filled.GetComponent<RectTransform>();
                if (slotRT != null && filledRT != null)
                {
                    filledRT.anchorMin = new Vector2(0.5f, 0.5f);
                    filledRT.anchorMax = new Vector2(0.5f, 0.5f);
                    filledRT.pivot = new Vector2(0.5f, 0.5f);

                    Vector3 worldPos = slotRT.position;
                    worldPos.y -= 150f; // tweak this offset for UI spacing
                    filledRT.position = worldPos;
                    filledRT.localScale = Vector3.one;
                }

                filled.alpha = expected;

                var img = filled.GetComponentInChildren<Image>();
                if (img != null)
                    img.color = expected.color;

                var text = filled.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                    text.text = expected.character;


                var cg2 = filled.GetComponent<CanvasGroup>() ?? filled.gameObject.AddComponent<CanvasGroup>();
                cg2.blocksRaycasts = false;
                cg2.interactable = false;
                cg2.alpha = 1f;

                usedInSlots.Add(expected);

                Debug.Log($"🟢 Prefilled Slot {i} with '{expected.character}'");

                if (onCorrectLetterPlaced != null)
                {
                    onCorrectLetterPlaced.Raise(expected);
                    Debug.Log($"📢 Raised OnCorrectLetterPlaced for prefilled letter: {expected.character}");
                }
            }
            */
            // 3️⃣ Show missing + distractors
            foreach (var alpha in alphabetObjects)
            {
                if (alpha == null) continue;

                bool used = alpha.alpha != null && usedInSlots.Contains(alpha.alpha);
                if (used)
                {
                    alpha.gameObject.SetActive(true);
                    continue;
                }

                alpha.ResetToOriginal();
                alpha.gameObject.SetActive(true);
                alpha.enabled = true;

                var cg = alpha.GetComponent<CanvasGroup>() ?? alpha.gameObject.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }

        // 4️⃣ Reposition existing alphabets (Option1, Option2, etc.) to their respective slot
        /*foreach (var alpha in alphabetObjects)
        {
            if (alpha == null || alpha.alpha == null)
                continue;

            // find matching slot that expects this alphabet
            AlphabetSlot matchingSlot = null;
            foreach (var slot in alphabetSlots)
            {
                if (slot == null || slot.ExpectedLetter == null)
                    continue;

                if (slot.ExpectedLetter == alpha.alpha)
                {
                    matchingSlot = slot;
                    break;
                }
            }

            if (matchingSlot != null)
            {
                RectTransform alphaRT = alpha.GetComponent<RectTransform>();
                RectTransform slotRT = matchingSlot.GetComponent<RectTransform>();

                // make alpha a child of slot (without keeping world position)
                alphaRT.SetParent(slotRT, false);

                // ensure it centers perfectly inside slot
                alphaRT.anchorMin = new Vector2(0.5f, 0.5f);
                alphaRT.anchorMax = new Vector2(0.5f, 0.5f);
                alphaRT.pivot = new Vector2(0.5f, 0.5f);
                alphaRT.anchoredPosition = Vector2.zero;
                alphaRT.localScale = Vector3.one;
                alphaRT.localRotation = Quaternion.identity;

                alpha.enabled = false;
                var cg = alpha.GetComponent<CanvasGroup>() ?? alpha.gameObject.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = false;
                cg.interactable = false;

                Debug.Log($"📦 '{alpha.name}' moved under slot '{matchingSlot.name}' successfully!");
            }
        }
        */
        Debug.Log("✅ FillInBlank visuals ready.");
    }
    private void ShowAllLetters()
    {
        foreach (var a in alphabetObjects)
            if (a != null)
                a.gameObject.SetActive(true);
    }

    private void HideAllLetters()
    {
        foreach (var s in alphabetSlots)
            if (s != null)
                s.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    private void ResetScene()
    {
        // Reset pool alphabets to original parents/positions and hide
        foreach (var a in alphabetObjects)
        {
            if (a == null) continue;
            a.ResetToOriginal();
            a.gameObject.SetActive(false);
        }

        // Clear slot visuals & text and destroy any cloned children
        foreach (var s in alphabetSlots)
        {
            if (s == null) continue;
            var tmp = s.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null) tmp.text = "";

            // destroy previous visual alphabets in slots
            var childrenToDestroy = new List<Transform>();
            foreach (Transform c in s.transform)
                if (c.GetComponent<Alphabet>() != null)
                    childrenToDestroy.Add(c);
            foreach (var t in childrenToDestroy) Destroy(t.gameObject);
        }
    }
}
