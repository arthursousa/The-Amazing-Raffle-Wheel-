using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionWheel : MonoBehaviour
{

    [Header("Balance")]
    public float wheelBaseAngle = -90;
    public float wheelIconDistance;
    public float dividerDistance;
    public float selectorRadius;
    public float baseFade;
    public float baseScale;

    [Header("References")]
    public CanvasGroup wheelGroup;
    public Transform wheelPieceHolder;
    public Transform iconHolder;
    public Transform dividerHolder;
    public Transform selector;
    public GameObject wheelPiece;
    public GameObject divider;
    public GameObject iconPlaceHolder;
    public GameObject crosshair;
    public GameObject selectorReferenceInterior;
    public GameObject selectorReferenceExterior;
    public GameObject exteriorIndiator;
    public Transform wheelRotator;
    public GameObject leftArmEquippedIconHolder;
    public GameObject rightArmEquippedIconHolder;
    public List<Transform> leftSavedSlotIconHolders;
    public List<Transform> rightSavedSlotIconHolders;

    public bool isSelectingArm = false;

    [HideInInspector]
    public List<ArmSaveSlot> savedSlots;

    [System.Serializable]
    public class ArmSaveSlot
    {
        public int left;
        public int right;

        public ArmSaveSlot(int l = 0, int r = 0)
        {
            left = l;
            right = r;
        }
    }

    public bool ArmSelectionLock = false;

    public static SelectionWheel INSTANCE;


    //NEW VARIABLES FOR THE WHEEL THING
    public bool IS_TESTING;
    public int testingAmmount;
    public int totalRaffleWeight = 1;
    public int ammountOfColors = 5;
    public float minHue;
    public float maxHue;
    public float colorIntensity;
    public float colorBrightness;
    public TextMeshProUGUI selectedText;
    public float minInitialSelectorSpeed;
    public float maxInitialSelectorSpeed;
    public float slowDowRate = 0.95f;
    public bool useExterior;
    public List<Transform> weightedPieces = new List<Transform>();
    public CanvasGroup group;
    private void Awake()
    {
        INSTANCE = this;

    }

    private void Start()
    {
        group.alpha = IS_TESTING ? 1 : 0;
    }


    // Update is called once per frame
    //void Update()
    //{
    //    if (GameManager.INSTANCE.currentUIPanelState == e_UIPanelState.playing)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Q) && !ArmSelectionLock)
    //        {
    //            StartCoroutine(ArmSelectionCoroutine());
    //        }

    //        if (!isSelectingArm && !ArmSelectionLock)
    //        {

    //            if (Input.GetButtonDown("Selection1"))
    //            {
    //                EquipSavedSlot(0);
    //            }
    //            else if (Input.GetButtonDown("Selection2"))
    //            {
    //                EquipSavedSlot(1);
    //            }
    //            else if (Input.GetButtonDown("Selection3"))
    //            {
    //                EquipSavedSlot(2);
    //            }
    //            else if (Input.GetButtonDown("Selection4"))
    //            {
    //                EquipSavedSlot(3);
    //            }
    //        }
    //    }
    //}

    private void Update()
    {
        if (IS_TESTING)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpinWheel();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                StopAllCoroutines();
                SetUpWheel();
            }
        }
    }

    public void SpinWheel()
    {
        Debug.Log("weeee");
        StartCoroutine(SpinWheelCoroutine());
    }

    public IEnumerator SpinWheelCoroutine()
    {
        bool isSpining = true;
        float auxAngle;
        int currentlySelectedIndex;
        GameObject currentReference = useExterior ? selectorReferenceExterior : selectorReferenceInterior;

        Transform currentSpinnable = useExterior ? wheelRotator : selector;
        float currentSpeed = Random.Range(minInitialSelectorSpeed, maxInitialSelectorSpeed);
        while (currentSpeed > 0)
        {
            //DO SELECTOR ROTATION HERE HOWEVER WE DO IT 
            if (currentSpeed <= 0)
                currentSpeed = 0;
            currentSpinnable.Rotate(new Vector3(0, 0, currentSpeed * Time.deltaTime));
            currentSpeed -= slowDowRate * Time.deltaTime;
            // selectorReference.transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * selectorSpeed * Time.deltaTime;

            //if (Vector3.Distance(selector.transform.position, selectorReference.transform.position) > selectorRadius)
            //{
            //    selectorReference.transform.position = selector.transform.position + (selectorReference.transform.position - selector.transform.position).normalized * selectorRadius;
            //}

            auxAngle = Vector3.Angle(Vector3.up, currentReference.transform.position - selector.transform.position);

            auxAngle = currentReference.transform.position.x < Screen.width / 2 ? auxAngle : 360 - auxAngle;

            auxAngle += (currentWheelDivisionAngle / 2);
            auxAngle = auxAngle % 360;

            foreach (Transform child in wheelPieceHolder)
            {
                // child.GetComponent<Image>().DOFade(baseFade, 0);
                child.transform.localScale = Vector3.one * baseScale;
            }

            currentlySelectedIndex = (int)Mathf.Floor((auxAngle / currentWheelDivisionAngle));

            currentlySelectedIndex = Mathf.Clamp(currentlySelectedIndex, 0, weightedPieces.Count);

            if (currentlySelectedIndex != 0)
                currentlySelectedIndex = useExterior ? weightedPieces.Count - currentlySelectedIndex : currentlySelectedIndex;

            //   wheelPieceHolder.GetChild(currentlySelectedIndex).GetComponent<Image>().DOFade(1, 0);
            weightedPieces[currentlySelectedIndex].localScale = Vector3.one * baseScale * 1.05f;
            selectedText.text = weightedPieces[currentlySelectedIndex].GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;

            //CheckArmOnInput(ThirdPersonUserController.INSTANCE.m_leftDown, ThirdPersonUserController.INSTANCE.m_rightDown);


            //if (Input.GetButtonDown("Selection1"))
            //{
            //    SaveSlot(0, selectedLeft, selectedRight);
            //}
            //else if (Input.GetButtonDown("Selection2"))
            //{
            //    SaveSlot(1, selectedLeft, selectedRight);
            //}
            //else if (Input.GetButtonDown("Selection3"))
            //{
            //    SaveSlot(2, selectedLeft, selectedRight);
            //}
            //else if (Input.GetButtonDown("Selection4"))
            //{
            //    SaveSlot(3, selectedLeft, selectedRight);
            //}

            Debug.Log(currentlySelectedIndex);
            // Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }
        if (!IS_TESTING)
        {
            yield return new WaitForSecondsRealtime(1.5f);

            CommandHandler.INSTANCE.WheelWin(selectedText.text);
            CommandHandler.INSTANCE.ResetDrawing();
            yield return new WaitForSecondsRealtime(3);
            group.alpha = 0;
        }
    }

    private float currentWheelDivisionAngle;
    private GameObject newWheelPieceAux;
    public void SetUpWheel()
    {
        foreach (Transform child in wheelPieceHolder)
        {
            Destroy(child.gameObject);
        }

        if (!IS_TESTING)
        {
            totalRaffleWeight = 0;
            foreach (var entry in CommandHandler.INSTANCE.currentRaffleWeights)
                totalRaffleWeight += entry.Value;
        }
        else
        {
            CommandHandler.INSTANCE.currentRaffleWeights = new Dictionary<string, int>();
            for (int j = 0; j < testingAmmount; j++)
            {
                CommandHandler.INSTANCE.currentRaffleWeights.Add("TheBookSnail_" + j.ToString(), Random.Range(1, 20));
                Debug.LogFormat("{0}  {1}", "TheBookSnail_" + j.ToString(), CommandHandler.INSTANCE.currentRaffleWeights["TheBookSnail_" + j.ToString()]);
            }
            totalRaffleWeight = 0;

            foreach (var entry in CommandHandler.INSTANCE.currentRaffleWeights)
                totalRaffleWeight += entry.Value;
        }
        if (totalRaffleWeight == 0)
            return;

        currentWheelDivisionAngle = (float)((float)360 / (float)totalRaffleWeight);

        int i = 0;
        int color = 0;
        weightedPieces = new List<Transform>();
        wheelRotator.rotation = Quaternion.Euler(0, 0, 0);
        selector.rotation = Quaternion.Euler(0, 0, 0);
        float offset = 0;
        foreach (var entry in CommandHandler.INSTANCE.currentRaffleWeights)
        {
            i += entry.Value;
            if (color == 0)
            {
                i = 0;
                if (entry.Value % 2 != 0)
                {
                    offset = (entry.Value - 1) / 2;
                    //wheelRotator.Rotate(new Vector3(0, 0, -offset * currentWheelDivisionAngle));
                }
                else
                    offset = (entry.Value - 1f) / 2;
                // offset = (entry.Value - 1.125f) / ;

                wheelRotator.Rotate(new Vector3(0, 0, -offset * currentWheelDivisionAngle));
            }
            newWheelPieceAux = Instantiate(wheelPiece, wheelPieceHolder.transform.position, Quaternion.Euler(0, 0, (i + offset) * currentWheelDivisionAngle + 180 * (1 + (currentWheelDivisionAngle * 1f / 360))), wheelPieceHolder);
            newWheelPieceAux.GetComponent<Image>().fillAmount = (currentWheelDivisionAngle * entry.Value / 360) * 1f;

            // newWheelPieceAux.GetComponent<Image>().color = Random.ColorHSV(0,0.5f,1,1,0.5f,0.5f);
            newWheelPieceAux.GetComponent<Image>().color = Color.HSVToRGB(minHue + (maxHue - minHue) * ((color % (float)ammountOfColors) / ammountOfColors), colorIntensity, colorBrightness);

            newWheelPieceAux.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, -180 - newWheelPieceAux.GetComponent<Image>().fillAmount * 180);

            newWheelPieceAux.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = entry.Key;

            for (int k = 0; k < entry.Value; k++)
            {
                weightedPieces.Add(newWheelPieceAux.transform);
            }

            color++;
        }

    }

    //private float auxAngle;
    //private int currentlySelectedIndex;
    //private int selectedLeft = 0;
    //private int selectedRight = 0;
    //public int currentLeft = -1;
    //public int currentRight = -1;

    //private IEnumerator ArmSelectionCoroutine()
    //{

    //    isSelectingArm = true;

    //    // SetUpWheel();
    //    ThirdPersonUserController.INSTANCE.cameraControlLock = true;
    //    ThirdPersonUserController.INSTANCE.armLock = true;

    //    crosshair.SetActive(false);

    //    wheelGroup.PlayEnablingAnimation(eUI_TransitionType.FADE, 0.025f);

    //    Time.timeScale = 0.1f;

    //    selectorReference.transform.position = wheelPieceHolder.transform.position;
    //    selector.rotation = Quaternion.identity;

    //    selectedLeft = currentLeft;
    //    selectedRight = currentRight;

    //    while (Input.GetKey(KeyCode.Q) && GameManager.INSTANCE.currentUIPanelState == e_UIPanelState.playing)
    //    {

    //        selectorReference.transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * selectorSpeed * Time.deltaTime;

    //        if (Vector3.Distance(selector.transform.position, selectorReference.transform.position) > selectorRadius)
    //        {
    //            selectorReference.transform.position = selector.transform.position + (selectorReference.transform.position - selector.transform.position).normalized * selectorRadius;
    //        }

    //        auxAngle = Vector3.Angle(Vector3.up, selectorReference.transform.position - selector.transform.position);

    //        auxAngle = selectorReference.transform.position.x < Screen.width / 2 ? auxAngle : 360 - auxAngle;
    //        selector.rotation = Quaternion.Euler(0, 0, auxAngle);

    //        auxAngle += (currentWheelDivisionAngle / 2);
    //        auxAngle = auxAngle % 360;

    //        foreach (Transform child in wheelPieceHolder)
    //        {
    //            child.GetComponent<Image>().DOFade(baseFade, 0);
    //            child.transform.localScale = Vector3.one * baseScale;
    //        }

    //        currentlySelectedIndex = (int)Mathf.Floor((auxAngle / currentWheelDivisionAngle));

    //        currentlySelectedIndex = Mathf.Clamp(currentlySelectedIndex, 0, wheelPieceHolder.childCount - 1);

    //        wheelPieceHolder.GetChild(currentlySelectedIndex).GetComponent<Image>().DOFade(1, 0);
    //        wheelPieceHolder.GetChild(currentlySelectedIndex).transform.localScale = Vector3.one * baseScale * 1.05f;


    //        CheckArmOnInput(ThirdPersonUserController.INSTANCE.m_leftDown, ThirdPersonUserController.INSTANCE.m_rightDown);


    //        if (Input.GetButtonDown("Selection1"))
    //        {
    //            SaveSlot(0, selectedLeft, selectedRight);
    //        }
    //        else if (Input.GetButtonDown("Selection2"))
    //        {
    //            SaveSlot(1, selectedLeft, selectedRight);
    //        }
    //        else if (Input.GetButtonDown("Selection3"))
    //        {
    //            SaveSlot(2, selectedLeft, selectedRight);
    //        }
    //        else if (Input.GetButtonDown("Selection4"))
    //        {
    //            SaveSlot(3, selectedLeft, selectedRight);
    //        }


    //        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    //        yield return null;
    //    }


    //    if (selectedLeft != currentLeft)
    //    {
    //        if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedLeft].slotType == ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //        {
    //            ArmExtensionHandler.INSTANCE.SwitchDoubleArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedLeft], selectedLeft);
    //        }
    //        else
    //        {
    //            ArmExtensionHandler.INSTANCE.SwitchArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedLeft], MagniAnimator.INSTANCE.arms[0], selectedLeft);
    //        }
    //        // currentLeft = selectedLeft;
    //    }

    //    if (selectedRight != currentRight)
    //    {
    //        if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedRight].slotType != ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //        {
    //            ArmExtensionHandler.INSTANCE.SwitchArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedRight], MagniAnimator.INSTANCE.arms[1], selectedRight);
    //        }
    //        // currentRight = selectedRight;
    //    }



    //    Time.timeScale = 1f;
    //    Time.fixedDeltaTime = Time.timeScale * 0.02f;
    //    wheelGroup.PlayDisablingAnimation(eUI_TransitionType.FADE, 0.025f);

    //    ThirdPersonUserController.INSTANCE.cameraControlLock = false;
    //    ThirdPersonUserController.INSTANCE.armLock = false;
    //    crosshair.SetActive(true);

    //    isSelectingArm = false;
    //    yield return null;
    //}

    //private void CheckArmOnInput(bool leftInput, bool rightInput)
    //{
    //    int selectedIndex = currentlySelectedIndex;

    //    if (leftInput || rightInput)
    //    {
    //        switch (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[currentlySelectedIndex].slotType)
    //        {
    //            case ArmExtensionData.e_ArmExtendionSlotType.singleArm:

    //                if (leftInput)
    //                {
    //                    if (selectedLeft == currentlySelectedIndex)
    //                        return;

    //                    if (selectedRight == currentlySelectedIndex)
    //                    {
    //                        selectedRight = selectedLeft;
    //                        selectedLeft = currentlySelectedIndex;
    //                    }
    //                    else
    //                    {
    //                        selectedLeft = currentlySelectedIndex;
    //                    }

    //                    if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedRight].slotType == ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //                    {
    //                        selectedRight = 0;
    //                    }
    //                }

    //                if (rightInput)
    //                {
    //                    if (selectedRight == currentlySelectedIndex)
    //                        return;

    //                    if (selectedLeft == currentlySelectedIndex)
    //                    {
    //                        selectedLeft = selectedRight;
    //                        selectedRight = currentlySelectedIndex;
    //                    }
    //                    else
    //                    {
    //                        selectedRight = currentlySelectedIndex;
    //                    }

    //                    if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedLeft].slotType == ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //                    {
    //                        selectedLeft = 0;
    //                    }
    //                }

    //                break;
    //            case ArmExtensionData.e_ArmExtendionSlotType.doubleArm:

    //                if (leftInput || rightInput)
    //                {
    //                    selectedLeft = currentlySelectedIndex;
    //                    selectedRight = currentlySelectedIndex;
    //                }

    //                break;
    //            case ArmExtensionData.e_ArmExtendionSlotType.multipleArm:

    //                if (leftInput)
    //                {
    //                    selectedLeft = currentlySelectedIndex;

    //                    if (selectedRight >= 0 && ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedRight].slotType == ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //                    {
    //                        selectedRight = 0;
    //                    }
    //                }

    //                if (rightInput)
    //                {
    //                    selectedRight = currentlySelectedIndex;

    //                    if (selectedLeft >= 0 && ArmExtensionHandler.INSTANCE.avaliableArmExtensions[selectedLeft].slotType == ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //                    {
    //                        selectedLeft = 0;
    //                    }

    //                }

    //                break;
    //            default:
    //                break;
    //        }

    //        if (selectedRight != -1)
    //            EquipIcon(selectedRight, rightArmEquippedIconHolder.transform);
    //        if (selectedLeft != -1)
    //            EquipIcon(selectedLeft, leftArmEquippedIconHolder.transform);
    //    }

    //}

    //private void EquipIcon(int index, Transform iconHolder)
    //{
    //    foreach (Transform child in iconHolder)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[index].icon == null)
    //    {
    //        Instantiate(iconPlaceHolder, iconHolder.position, Quaternion.identity, iconHolder);
    //    }

    //    else
    //    {
    //        Instantiate(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[index].icon, iconHolder.position, Quaternion.identity, iconHolder);
    //    }
    //}

    //public void EquipSavedSlot(int slotIndex)
    //{
    //    if (savedSlots[slotIndex].left != -1)
    //    {

    //        if (ArmExtensionHandler.INSTANCE.avaliableArmExtensions[savedSlots[slotIndex].left].slotType != ArmExtensionData.e_ArmExtendionSlotType.doubleArm)
    //        {
    //            if (currentLeft != savedSlots[slotIndex].left)
    //            {
    //                ArmExtensionHandler.INSTANCE.SwitchArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[savedSlots[slotIndex].left], MagniAnimator.INSTANCE.arms[0], savedSlots[slotIndex].left);
    //                //currentLeft = savedSlots[slotIndex].left;
    //                EquipIcon(savedSlots[slotIndex].left, leftArmEquippedIconHolder.transform);
    //            }

    //            if (currentRight != savedSlots[slotIndex].right)
    //            {
    //                ArmExtensionHandler.INSTANCE.SwitchArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[savedSlots[slotIndex].right], MagniAnimator.INSTANCE.arms[1], savedSlots[slotIndex].right);
    //                //currentRight = savedSlots[slotIndex].right;
    //                EquipIcon(savedSlots[slotIndex].right, rightArmEquippedIconHolder.transform);
    //            }
    //        }

    //        else if (currentLeft != savedSlots[slotIndex].left)
    //        {
    //            ArmExtensionHandler.INSTANCE.SwitchDoubleArmExtension(ArmExtensionHandler.INSTANCE.avaliableArmExtensions[savedSlots[slotIndex].left], savedSlots[slotIndex].left);
    //            //  currentLeft = savedSlots[slotIndex].left;
    //            EquipIcon(savedSlots[slotIndex].left, leftArmEquippedIconHolder.transform);
    //            //  currentRight = savedSlots[slotIndex].left;
    //            EquipIcon(savedSlots[slotIndex].right, rightArmEquippedIconHolder.transform);
    //        }
    //    }
    //}

    //public void SaveSlot(int slotIndex, int left, int right)
    //{
    //    // Debug.bre
    //    savedSlots[slotIndex].left = left;
    //    savedSlots[slotIndex].right = right;

    //    EquipIcon(left, leftSavedSlotIconHolders[slotIndex].transform);
    //    EquipIcon(right, rightSavedSlotIconHolders[slotIndex].transform);
    //}
}
