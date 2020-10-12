using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum FlipMode
{
    RightToLeft,
    LeftToRight
}

[ExecuteInEditMode]
public class Book : MonoBehaviour
{
    public Canvas canvas;
    [SerializeField]
    private RectTransform bookPanel;
    public Sprite[] bookPages;
    public bool interactable = true;

    

    //represent the index of the sprite shown in the right page
    public int currentPage;
    public int TotalPageCount => bookPages.Length;

    public Vector3 EndBottomLeft => _ebl;

    public Vector3 EndBottomRight => _ebr;

    public float Height => bookPanel.rect.height;

    public Image ClippingPlane;
    public Image NextPageClip;
    
    public Image LeftMask;
    public Image LeftNext;
    public Image RightMask;
    public Image RightNext;
    public UnityEvent OnFlip;
    
    [Header("Shadows")]
    public bool enableShadowEffect = true;
    public Image Shadow;
    public Image ShadowLTR;
    
    //Spine Bottom
    private Vector3 _sb;

    //Spine Top
    private Vector3 _st;

    //corner of the page
    private Vector3 _c;

    //Edge Bottom Right
    private Vector3 _ebr;

    //Edge Bottom Left
    private Vector3 _ebl;

    //follow point 
    private Vector3 _followPoint;

    //current flip mode
    private FlipMode _mode;
    private bool _pageDragging;
    private float _radius1, _radius2;
    private Coroutine _currentCoroutine;
    
    private readonly Sprite _background = null;

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (bookPanel == null)
            bookPanel = GetComponent<RectTransform>();
        
        var scaleFactor = canvas.scaleFactor;

        var boolPanelRect = bookPanel.rect; 
        var pageWidth = (boolPanelRect.width * scaleFactor - 1) / 2;
        var pageHeight = boolPanelRect.height * scaleFactor;
        
        LeftMask.gameObject.SetActive(false);
        RightMask.gameObject.SetActive(false);
        
        UpdateSprites();

        var bookPanelTransformPosition = bookPanel.transform.position;
        var globalsb = bookPanelTransformPosition + new Vector3(0, -pageHeight / 2);
        _sb = GetTransformPoint(globalsb);
        var globalebr = bookPanelTransformPosition + new Vector3(pageWidth, -pageHeight / 2);
        _ebr = GetTransformPoint(globalebr);
        var globalebl = bookPanelTransformPosition + new Vector3(-pageWidth, -pageHeight / 2);
        _ebl = GetTransformPoint(globalebl);
        var globalst = bookPanelTransformPosition + new Vector3(0, pageHeight / 2);
        _st = GetTransformPoint(globalst);
        
        _radius1 = Vector2.Distance(_sb, _ebr);
        
        var scaledPageWidth = pageWidth / scaleFactor;
        var scaledPageHeight = pageHeight / scaleFactor;
        _radius2 = Mathf.Sqrt(scaledPageWidth * scaledPageWidth + scaledPageHeight * scaledPageHeight);
        
        ClippingPlane.rectTransform.sizeDelta =
            new Vector2(scaledPageWidth * 2, scaledPageHeight + scaledPageWidth * 2);
        
        Shadow.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
        ShadowLTR.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
        NextPageClip.rectTransform.sizeDelta = new Vector2(scaledPageWidth, scaledPageHeight + scaledPageWidth * 0.6f);
    }

    private Vector3 GetTransformPoint(Vector3 global)
    {
        return bookPanel.InverseTransformPoint(global);
    }

    private void Update()
    {
        if (_pageDragging && interactable)
        {
            UpdateBook();
        }
    }

    public void UpdateBook()
    {
        _followPoint = Vector3.Lerp(_followPoint, GetTransformPoint(Input.mousePosition), Time.deltaTime * 10);
        if (_mode == FlipMode.RightToLeft)
            UpdateBookRtlToPoint(_followPoint);
        else
            UpdateBookLtrToPoint(_followPoint);
    }

    public void UpdateBookLtrToPoint(Vector3 followLocation)
    {
        _mode = FlipMode.LeftToRight;
        _followPoint = followLocation;
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
        ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);
        LeftMask.transform.SetParent(ClippingPlane.transform, true);

        RightMask.transform.SetParent(bookPanel.transform, true);
        LeftNext.transform.SetParent(bookPanel.transform, true);

        _c = Calc_C_Position(followLocation);
        Vector3 t1;
        float T0_T1_Angle = Calc_T0_T1_Angle(_c, _ebl, out t1);
        if (T0_T1_Angle < 0) T0_T1_Angle += 180;

        ClippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
        ClippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        LeftMask.transform.position = bookPanel.TransformPoint(_c);
        float C_T1_dy = t1.y - _c.y;
        float C_T1_dx = t1.x - _c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        LeftMask.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle - 180);

        NextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle - 90);
        NextPageClip.transform.position = bookPanel.TransformPoint(t1);
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        RightMask.transform.SetParent(ClippingPlane.transform, true);
        RightMask.transform.SetAsFirstSibling();

        ShadowLTR.rectTransform.SetParent(LeftMask.rectTransform, true);
    }

    public void UpdateBookRtlToPoint(Vector3 followLocation)
    {
        _mode = FlipMode.RightToLeft;
        _followPoint = followLocation;

        if (enableShadowEffect)
        {
            var shadowTransform = Shadow.transform; 
            shadowTransform.SetParent(ClippingPlane.transform, true);
            shadowTransform.localPosition = new Vector3(0, 0, 0);
            shadowTransform.localEulerAngles = new Vector3(0, 0, 0);
        }
        RightMask.transform.SetParent(ClippingPlane.transform, true);

        LeftMask.transform.SetParent(bookPanel.transform, true);
        RightNext.transform.SetParent(bookPanel.transform, true);
        _c = Calc_C_Position(followLocation);
        Vector3 t1;
        float T0_T1_Angle = Calc_T0_T1_Angle(_c, _ebr, out t1);
        if (T0_T1_Angle >= -90) T0_T1_Angle -= 180;

        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        ClippingPlane.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
        ClippingPlane.transform.position = bookPanel.TransformPoint(t1);

        //page position and angle
        RightMask.transform.position = bookPanel.TransformPoint(_c);
        float C_T1_dy = t1.y - _c.y;
        float C_T1_dx = t1.x - _c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        RightMask.transform.eulerAngles = new Vector3(0, 0, C_T1_Angle);

        NextPageClip.transform.eulerAngles = new Vector3(0, 0, T0_T1_Angle + 90);
        NextPageClip.transform.position = bookPanel.TransformPoint(t1);
        RightNext.transform.SetParent(NextPageClip.transform, true);
        LeftMask.transform.SetParent(ClippingPlane.transform, true);
        LeftMask.transform.SetAsFirstSibling();

        Shadow.rectTransform.SetParent(RightMask.rectTransform, true);
    }

    private float Calc_T0_T1_Angle(Vector3 c, Vector3 bookCorner, out Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        float T0_CORNER_dy = bookCorner.y - t0.y;
        float T0_CORNER_dx = bookCorner.x - t0.x;
        float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
        float T0_T1_Angle = 90 - T0_CORNER_Angle;

        float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
        T1_X = normalizeT1X(T1_X, bookCorner, _sb);
        t1 = new Vector3(T1_X, _sb.y, 0);
        ////////////////////////////////////////////////
        //clipping plane angle=T0_T1_Angle
        float T0_T1_dy = t1.y - t0.y;
        float T0_T1_dx = t1.x - t0.x;
        T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
        return T0_T1_Angle;
    }

    private float normalizeT1X(float t1, Vector3 corner, Vector3 sb)
    {
        if (t1 > sb.x && sb.x > corner.x)
            return sb.x;
        if (t1 < sb.x && sb.x < corner.x)
            return sb.x;
        return t1;
    }

    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        _followPoint = followLocation;
        float F_SB_dy = _followPoint.y - _sb.y;
        float F_SB_dx = _followPoint.x - _sb.x;
        float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        Vector3 r1 = new Vector3(_radius1 * Mathf.Cos(F_SB_Angle), _radius1 * Mathf.Sin(F_SB_Angle), 0) + _sb;

        float F_SB_distance = Vector2.Distance(_followPoint, _sb);
        if (F_SB_distance < _radius1)
            c = _followPoint;
        else
            c = r1;
        float F_ST_dy = c.y - _st.y;
        float F_ST_dx = c.x - _st.x;
        float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
        Vector3 r2 = new Vector3(_radius2 * Mathf.Cos(F_ST_Angle),
            _radius2 * Mathf.Sin(F_ST_Angle), 0) + _st;
        float C_ST_distance = Vector2.Distance(c, _st);
        if (C_ST_distance > _radius2)
            c = r2;
        return c;
    }

    /// <summary>
    /// Начало перетаскивания правой страницы влево
    /// </summary>
    public void BeginDragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Length) 
            return;
        
        _pageDragging = true;
        _mode = FlipMode.RightToLeft;
        _followPoint = point;


        NextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        LeftMask.gameObject.SetActive(true);
        LeftMask.rectTransform.pivot = new Vector2(0, 0);
        LeftMask.transform.position = RightNext.transform.position;
        LeftMask.transform.eulerAngles = new Vector3(0, 0, 0);
        LeftMask.transform.SetAsFirstSibling();

        RightMask.gameObject.SetActive(true);
        RightMask.transform.position = RightNext.transform.position;
        RightMask.transform.eulerAngles = new Vector3(0, 0, 0);
        
        //ToDo: Set objects
        LeftMask.sprite = (currentPage < bookPages.Length) 
            ? bookPages[currentPage] 
            : _background;
        RightMask.sprite = (currentPage < bookPages.Length - 1) 
            ? bookPages[currentPage + 1] 
            : _background;
        RightNext.sprite = (currentPage < bookPages.Length - 2) 
            ? bookPages[currentPage + 2] 
            : _background;

        LeftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) Shadow.gameObject.SetActive(true);
        UpdateBookRtlToPoint(_followPoint);
    }

    /// <summary>
    /// UnityEvent: Call BeginDragRightPageToPoint
    /// </summary>
    public void OnMouseDragRightPage()
    {
        if (interactable)
            BeginDragRightPageToPoint(GetTransformPoint(Input.mousePosition));
    }

    /// <summary>
    /// Начало перетаскивания левой страницы вправо
    /// </summary>
    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        _pageDragging = true;
        _mode = FlipMode.LeftToRight;
        _followPoint = point;

        NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        RightMask.gameObject.SetActive(true);
        RightMask.transform.position = LeftNext.transform.position;
        RightMask.transform.eulerAngles = new Vector3(0, 0, 0);
        RightMask.transform.SetAsFirstSibling();

        LeftMask.gameObject.SetActive(true);
        LeftMask.rectTransform.pivot = new Vector2(1, 0);
        LeftMask.transform.position = LeftNext.transform.position;
        LeftMask.transform.eulerAngles = new Vector3(0, 0, 0);
        
        //ToDo: Set objects
        RightMask.sprite = bookPages[currentPage - 1];
        LeftMask.sprite = (currentPage >= 2) 
            ? bookPages[currentPage - 2] 
            : _background;
        LeftNext.sprite = (currentPage >= 3) 
            ? bookPages[currentPage - 3] 
            : _background;

        RightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
        UpdateBookLtrToPoint(_followPoint);
    }

    /// <summary>
    /// Unity Event: Call DragLeftPageToPoint
    /// </summary>
    public void OnMouseDragLeftPage()
    {
        if (interactable)
            DragLeftPageToPoint(GetTransformPoint(Input.mousePosition));
    }

    public void OnMouseRelease()
    {
        if (interactable)
            ReleasePage();
    }

    public void ReleasePage()
    {
        if (_pageDragging)
        {
            _pageDragging = false;
            float distanceToLeft = Vector2.Distance(_c, _ebl);
            float distanceToRight = Vector2.Distance(_c, _ebr);
            if (distanceToRight < distanceToLeft && _mode == FlipMode.RightToLeft)
                TweenBack();
            else if (distanceToRight > distanceToLeft && _mode == FlipMode.LeftToRight)
                TweenBack();
            else
                TweenForward();
        }
    }

    

    void UpdateSprites()
    {
        LeftNext.sprite = (currentPage > 0 && currentPage <= bookPages.Length)
            ? bookPages[currentPage - 1]
            : null;
        RightNext.sprite = (currentPage >= 0 && currentPage < bookPages.Length) 
            ? bookPages[currentPage] 
            : null;
    }

    public void TweenForward()
    {
        if (_mode == FlipMode.RightToLeft)
            _currentCoroutine = StartCoroutine(TweenTo(_ebl, 0.15f, () => { Flip(); }));
        else
            _currentCoroutine = StartCoroutine(TweenTo(_ebr, 0.15f, () => { Flip(); }));
    }

    void Flip()
    {
        if (_mode == FlipMode.RightToLeft)
            currentPage += 2;
        else
            currentPage -= 2;
        LeftNext.transform.SetParent(bookPanel.transform, true);
        LeftMask.transform.SetParent(bookPanel.transform, true);
        LeftNext.transform.SetParent(bookPanel.transform, true);
        LeftMask.gameObject.SetActive(false);
        RightMask.gameObject.SetActive(false);
        RightMask.transform.SetParent(bookPanel.transform, true);
        RightNext.transform.SetParent(bookPanel.transform, true);
        UpdateSprites();
        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);
        if (OnFlip != null)
            OnFlip.Invoke();
    }

    public void TweenBack()
    {
        if (_mode == FlipMode.RightToLeft)
        {
            _currentCoroutine = StartCoroutine(TweenTo(_ebr, 0.15f,
                () =>
                {
                    UpdateSprites();
                    RightNext.transform.SetParent(bookPanel.transform);
                    RightMask.transform.SetParent(bookPanel.transform);

                    LeftMask.gameObject.SetActive(false);
                    RightMask.gameObject.SetActive(false);
                    _pageDragging = false;
                }
            ));
        }
        else
        {
            _currentCoroutine = StartCoroutine(TweenTo(_ebl, 0.15f,
                () =>
                {
                    UpdateSprites();

                    LeftNext.transform.SetParent(bookPanel.transform);
                    LeftMask.transform.SetParent(bookPanel.transform);

                    LeftMask.gameObject.SetActive(false);
                    RightMask.gameObject.SetActive(false);
                    _pageDragging = false;
                }
            ));
        }
    }

    public IEnumerator TweenTo(Vector3 to, float duration, Action onFinish)
    {
        int steps = (int) (duration / 0.025f);
        Vector3 displacement = (to - _followPoint) / steps;
        for (int i = 0; i < steps - 1; i++)
        {
            if (_mode == FlipMode.RightToLeft)
                UpdateBookRtlToPoint(_followPoint + displacement);
            else
                UpdateBookLtrToPoint(_followPoint + displacement);

            yield return new WaitForSeconds(0.025f);
        }

        if (onFinish != null)
            onFinish();
    }
}