using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FakeTrail : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3[] _segmentPoses, _segmentV;
    [SerializeField] private int _trailPositionCount = 30;
    [SerializeField] private Transform targetDir; // targetDir objesinin Y oku nereye bakıyorsa oraya doğru bir kuyruk oluşturulacak.
    [SerializeField] private float _trailLength = 0.15f; // kuyruk uzunluğu
    [SerializeField] private float _smoothness = 0.15f; // takip yumuşaklığı
    [SerializeField] private bool _startDelay = false; // başlangıçta yavaşça uzasın mı?
    [SerializeField] private float _delayLength = 1; // uzama süresi
    void Awake()
    {
        // Line Renderer componentini atıyoruz.
        _lineRenderer = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        // Eğer spesifik bir obje belirtilmemişse bir obje oluşturuluyor ve y oku aşağıya doğru baktırılıyor.
        if (targetDir == null)
        {
            targetDir = Instantiate(new GameObject("targetDir"), transform.position, Quaternion.identity, transform).transform;
            targetDir.transform.rotation = Quaternion.Euler(0, 0, -180);
        }

        // başlangıçta kuyruğun yavaşça uzamasını istiyorsak...
        if (_startDelay)
        {
            // uzunluğu sıfırlamadan önce girilen değeri tutuyoruz.
            var temp = _trailLength;

            // yumuşak şekilde 0'dan girilen değere doğru delay süresince arttırıyoruz
            StartCoroutine(DoFade(temp, _delayLength));
        }

        // _trailPositionCount kadar nokta ekliyoruz. 30 çoğu durum için yeterli olacaktır
        _lineRenderer.positionCount = _trailPositionCount;

        // nokta uzunluğunda bir liste oluşturuyoruz. böylece sonradan istediğimiz noktanın değerleriyle oynayabileceğiz.
        _segmentPoses = new Vector3[_trailPositionCount];

        // noktaların velocity değerini tutmak için bir liste daha. Böylece noktalar daha yumuşak bir şekilde hareket ediyor.
        _segmentV = new Vector3[_trailPositionCount];
    }

    void Update()
    {
        // kuyruğun ilk pozisyonunu objenin ortasına ayarlıyoruz.
        _segmentPoses[0] = targetDir.position;

        for (int i = 1; i < _trailPositionCount; i++)
        {
            // kuyruktaki tüm noktalar (ilk nokta hariç), bir önceki noktayı yavaşça takip ediyor.
            _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i], _segmentPoses[i - 1] + targetDir.up * _trailLength, ref _segmentV[i], _smoothness);
        }

        // noktaları Line Renderer componentine uyguluyoruz.
        _lineRenderer.SetPositions(_segmentPoses);
    }
    private IEnumerator DoFade(float targetNumber, float time)
    {
        float t = 0; // zaman
        _trailLength = 0; // uzunluğu sıfırlıyoruz.
        while (t < time)
        {
            // yumuşak şekilde uzaması için kodlar.
            t += Time.fixedDeltaTime;
            yield return new WaitForSeconds(0.02f);
            _trailLength += targetNumber / time * Time.fixedDeltaTime;
        }
        // son uzunluğa kesin olarak erişmesi için atama.
        _trailLength = targetNumber;
    }
}
