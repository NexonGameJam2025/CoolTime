using Core.Scripts;
using Core.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuilding : UIBase
{
    [SerializeField] private Building targetBuildingPrefab;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private int cost = 0;
    
    private UI_EventHandler _eventHandler;
    private Building _spawnedBuilding;
    private TileNode _currentAttachedNode;
    private Wall _currentAttachedWall;
    private Vector3 _originRotation = Vector3.zero;
    private Vector3 _originScale = Vector3.one;
    
    private bool _isDragging = false;
    private bool _isAttached = false;
    
    private void Awake()
    {
        _eventHandler = Utils.GetOrAddComponent<UI_EventHandler>(gameObject);
        _eventHandler.OnPointerDownHandler += OnPointerDownAction;
        _eventHandler.OnPointerUpHandler += OnPointerUpAction;
        _eventHandler.OnDragHandler += OnDragAction;
        textCost.text = cost.ToString();
    }

    public void IncreaseCost()
    {
        cost += 2;
        textCost.text = cost.ToString();
    }

    private void OnPointerDownAction(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!GameManager.Instance.IsCanBuyBuilding(cost))
        {
            Utils.OnWrongSituationShake(transform, true, 2);
            return;
        }
        
        var mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        
        _spawnedBuilding = Instantiate(targetBuildingPrefab, mousePos, Quaternion.identity);
        _spawnedBuilding.OnDragging();
        _originRotation = _spawnedBuilding.transform.eulerAngles;
        _originScale = _spawnedBuilding.transform.localScale;
        _isDragging = true;
    }
    
    private void OnDragAction(PointerEventData eventData)
    {
        if (!_isDragging || _spawnedBuilding == null)
            return;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        var hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f);
        
        if (hit)
        {
            if (_spawnedBuilding.BuildingType == Define.EBuildingType.Wall)
            {
                var target = hit.transform.gameObject.GetComponent<Wall>();
                if (target != null && !target.IsEnable)
                {
                    _spawnedBuilding.transform.position = target.transform.position;
                    _spawnedBuilding.transform.eulerAngles = target.transform.eulerAngles;
                    _spawnedBuilding.transform.localScale = target.transform.localScale;
                    _currentAttachedWall = target;
                    _isAttached = true;
                    return;
                }
            }
            else
            {
                var target = hit.transform.gameObject.GetComponent<TileNode>();
                if (target != null && !target.OnBuilding)
                {
                    _spawnedBuilding.transform.position = target.transform.position;
                    _currentAttachedNode = target;
                    _isAttached = true;
                    return;
                }
            }
        }
        
        _spawnedBuilding.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        _spawnedBuilding.transform.eulerAngles = _originRotation;
        _spawnedBuilding.transform.localScale = _originScale;
        _currentAttachedNode = null;
        _currentAttachedWall = null;
        _isAttached = false;
    }
    
    private void OnPointerUpAction(PointerEventData eventData)
    {
        if (!_isDragging || _spawnedBuilding == null)
            return;

        if (_isAttached && GameManager.Instance.IsCanBuyBuilding(cost))
        {
            GameManager.Instance.AddGold(-cost);
            
            if (_spawnedBuilding.BuildingType == Define.EBuildingType.Wall)
            {
                _currentAttachedWall.OnFinishBuild();
                Destroy(_spawnedBuilding.gameObject);
            }
            else
            {
                var coordinate = _currentAttachedNode.Coordinate;
                _spawnedBuilding.OnBuilderStart(coordinate);
                _currentAttachedNode.SetBuilding(_spawnedBuilding);
            }
        }
        else
        {
            Destroy(_spawnedBuilding.gameObject);
        }
        
        _spawnedBuilding = null;
    }
}