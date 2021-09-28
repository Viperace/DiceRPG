using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class RollDiceViewTransform : MonoBehaviour
{
    Text numberText;
    [SerializeField] float movePosX = 2;
    [SerializeField] Transform movePosTarget;
    [SerializeField] float showDuration = 3f;
    [SerializeField] float delayStart = 0f;
    [SerializeField] float flashPeriod = 0.05f;
    [SerializeField] float baseJumpStrength = 1;

    [SerializeField] GameObject spawnToEpicentor;
    GameObject spawnFrom;
    GameParameter gameParameter;

    Vector3 origPos;
    void Awake()
    {
        DOTween.Init();

        numberText = GetComponentInChildren<Text>();

        // Save position
        origPos = this.transform.position;

        gameParameter = ScriptableObject.CreateInstance<GameParameter>();
    }

    void SetupSpawnPoint()
    {
        // Decide spawn posn
        if (this.CompareTag("PlayerDice"))
        {
            // Find represented slot
            // represented slot  tagged gear is dice, then spawn from this
            GearTextNumber[] arsenal = FindObjectsOfType<GearTextNumber>();
            GearDiceBehavior thisDice = GetComponent<GearDiceBehavior>();
            foreach (var item in arsenal)
                if (item.LinkedGearDice == thisDice.RepresentedDice)
                {
                    spawnFrom = item.gameObject;
                    break;
                }
        }
        else
        {
            GameObject enemyArsenal = GameObject.Find("EnemyArsenal");
            GearTextNumber[] gears = enemyArsenal.GetComponentsInChildren<GearTextNumber>();
            foreach (var item in gears)
            {
                spawnFrom = item.gameObject;
            }
        }
    }

    IEnumerator ShuffleNumber(int finalNumber, float duration, int min, int max, float durationVar = 1f)
    {
        // Start shuffling
        float endTime = Time.time + duration + durationVar;
        while (Time.time < endTime)
        {
            //numberText.text = Random.Range(min, max + 1).ToString();
            numberText.text = Random.Range(0, 10).ToString();
            yield return new WaitForSeconds(flashPeriod);
        }
        numberText.text = finalNumber.ToString();
    }


    void AnimateMoveDice(float delayStart, System.Action callback = null, float durationVar = 1f)
    {
        Sequence sequence = DOTween.Sequence();

        // Setup spawn point
        if (spawnFrom == null)
            SetupSpawnPoint();

        // Start point
        this.transform.position = new Vector3(spawnFrom.transform.position.x,
            spawnFrom.transform.position.y, this.transform.position.z);

        //---Define target pos
        //float targetPosX = transform.position.x + movePosX;
        //Vector3 targetPos = new Vector3(targetPosX, transform.position.y, transform.position.z);
        // Random child positions
        //Transform[] poss = spawnToEpicentor.GetComponentsInChildren<Transform>();
        //Vector3 targetPos = poss[Random.Range(0, poss.Length)].position;
        //targetPos = new Vector3(targetPos.x, targetPos.y, this.transform.position.z);

        Vector2 randPos = Random.insideUnitCircle * 0.2f;
        Vector3 targetPos = new Vector3(spawnToEpicentor.transform.position.x + randPos.x, spawnToEpicentor.transform.position.y + randPos.y, 0);
        targetPos = new Vector3(targetPos.x, targetPos.y, origPos.z);

        // Do move
        sequence.PrependInterval(delayStart);


        float showDurationWithVar = (showDuration + durationVar) * gameParameter.AnimationDurationMod;
        float jumpStrength = Random.Range(baseJumpStrength, baseJumpStrength*1.5f);
        int jumpNumber = Random.Range(2, 3 + 1);
        sequence.Append(transform.DOJump(targetPos, jumpStrength, jumpNumber, showDurationWithVar).SetEase(Ease.OutCubic));
        //sequence.Append(transform.DOMoveX(targetPos, showDuration).SetEase(Ease.OutCubic));
        //sequence.Join(transform.DOPunchRotation(new Vector3(10, 30, 0), showDuration, 100));

        float epsX = Random.value > 0.5f ? 1 : -1;
        float epsY = Random.value > 0.5f ? 1 : -1;
        float nX = 0;
        float nY = Random.Range(3, 7);
        sequence.Join(transform.DOLocalRotate(new Vector3(epsX * (360 * nX), 0, epsY * (360 * nY)), showDurationWithVar, RotateMode.FastBeyond360));
        sequence.Append(transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f));
        if(callback != null)
            sequence.AppendCallback(() => callback());
    }

    void AnimateMoveNumbersToTarget(float delayStart, Vector3 targetPos)
    {
        Sequence sequence = DOTween.Sequence();

        if (spawnFrom == null)
            SetupSpawnPoint();

        // Start point
        this.transform.position = new Vector3(spawnFrom.transform.position.x,
            spawnFrom.transform.position.y, this.transform.position.z);

        // Do move
        sequence.PrependInterval(delayStart);

        float jumpStrength = baseJumpStrength * Random.Range(1, 1.25f);
        int jumpNumber = Random.Range(2, 3 + 1);

        int xDir = Random.value > 0.5f ? 1 : -1;
        int zDir = Random.value > 0.5f ? 1 : -1;
        sequence.Append(transform.DOJump(targetPos, jumpStrength, jumpNumber, showDuration * gameParameter.AnimationDurationMod).SetEase(Ease.OutCubic));
        sequence.Insert(0, transform.DOLocalRotate(new Vector3(xDir * 360, 0, zDir * 360), showDuration * gameParameter.AnimationDurationMod, RotateMode.FastBeyond360));

    }

    public void AnimateJumpTo(Vector3 position)
    {
        float jumpStrength = baseJumpStrength * Random.Range(1, 1.25f);
        int jumpNumber = 1;
        Vector2 noise = Random.insideUnitCircle * 0.15f;
        this.transform.DOJump(position + new Vector3(noise.x, noise.y, 0), jumpStrength, jumpNumber, 0.25f).SetEase(Ease.OutCubic);
    }

    public void AnimateReset(System.Action callback = null)
    {
        Sequence sequence = DOTween.Sequence();

        // Config
        float duration = 2f;
        float delayStart = 0f;

        // Reset first
        this.transform.rotation.eulerAngles.Set(0, 0, 0);

        // Define target pos
        Vector3 targetPos = origPos;

        // Do move
        sequence.PrependInterval(delayStart);

        float jumpStrength = baseJumpStrength * Random.Range(1, 1.25f);
        int jumpNumber = Random.Range(1, 2 + 1);
        sequence.Append(this.transform.DOJump(targetPos, jumpStrength, jumpNumber, duration).SetEase(Ease.OutCubic));
        sequence.Join(this.transform.DOLocalRotate(new Vector3(180, 0, 180), duration));
        sequence.Append(this.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.01f));

        if (callback != null)
        {
            sequence.AppendInterval(0.1f);
            sequence.AppendCallback(() => callback());
        }
            
    }

    public void AnimateFreeRoll(int finalValue, System.Action callback = null)
    {
        float durationVar = Random.Range(-1.5f, 0.5f);
        AnimateMoveDice(delayStart, callback, 0);
        StartCoroutine(ShuffleNumber(finalValue, showDuration * gameParameter.AnimationDurationMod + delayStart, 0, 9, durationVar));
    }

    public void AnimateTargetPositionRoll(int finalValue)
    {
        //AnimateMoveNumbersToTarget(delayStart, new Vector3(movePosTarget.position.x, movePosTarget.position.y, this.transform.position.z));
        AnimateMoveNumbersToTarget(delayStart, new Vector3(movePosTarget.position.x, movePosTarget.position.y, movePosTarget.position.z));
        
        StartCoroutine(ShuffleNumber(finalValue, showDuration * gameParameter.AnimationDurationMod + delayStart, 0, 9));
    }

    public void AnimateAttackOtherDice(GameObject otherDice, System.Action attackerCallback = null)
    {
        Sequence sequence = DOTween.Sequence();

        // Start point / Target point
        Vector3 origPos = this.transform.position;
        Vector3 targetPos = otherDice.transform.position;

        // Do move
        float delayStart = 0;
        sequence.PrependInterval(delayStart);

        // Do anim for this 
        float eps = this.CompareTag("PlayerDice") ? -0.1f : 0.1f;
        Vector3 backwardPos = transform.position + new Vector3(eps, 0, 0);
        sequence.Append(transform.DOMove(backwardPos, 0.1f));
            sequence.Join(transform.DOLocalRotate(new Vector3(Random.Range(30, 45), Random.Range(30, 45), 0), 0.5f));
        sequence.Append(transform.DOJump(targetPos, 0.4f, 1, 0.2f));
            sequence.Join(transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f));
        sequence.AppendInterval(0.2f);
        sequence.Append(transform.DOJump(origPos, 0.25f, 1, 0.2f));
        if(attackerCallback != null)
            sequence.AppendCallback( () => { attackerCallback(); });

        // Do anim for target
        Sequence sequence2 = DOTween.Sequence();
        sequence2.PrependInterval(0.5f + 0.2f);
        sequence2.Append(otherDice.transform.DOShakePosition(0.5f, 15));
        sequence2.Join(otherDice.transform.DOShakeRotation(0.5f, 45, 5));
    }

    public void DoShake(float delay)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        sequence.Append(transform.DOShakePosition(0.5f, 1.5f));
        sequence.Join(transform.DOShakeRotation(0.5f, 45, 5));
    }

    public void _TestNumber()
    {
        int min = 1;
        int max = 15;
        int x = Random.Range(min, max + 1);
        AnimateMoveDice(delayStart);
        StartCoroutine(ShuffleNumber(x, showDuration + delayStart, min, max));
    }

    public void _TestEnemyNumber()
    {
        int min = 1;
        int max = 15;
        int x = Random.Range(min, max + 1);
        AnimateMoveNumbersToTarget(delayStart, new Vector3(movePosTarget.position.x, movePosTarget.position.y, this.transform.position.z));
        StartCoroutine(ShuffleNumber(x, showDuration + delayStart, min, max));
    }
}

