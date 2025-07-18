using Core.Scripts;
using Core.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuilding : UIBase
{
    [SerializeField] private Building targetBuildingPrefab;
    
    private UI_EventHandler _eventHandler;
    private Building _spawnedBuilding;
    private TileNode _currentAttachedNode;
    
    private bool _isDragging = false;
    private bool _isAttached = false;
    
    private void Awake()
    {
        _eventHandler = Utils.GetOrAddComponent<UI_EventHandler>(gameObject);
        _eventHandler.OnPointerDownHandler += OnPointerDownAction;
        _eventHandler.OnPointerUpHandler += OnPointerUpAction;
        _eventHandler.OnDragHandler += OnDragAction;
    }

    private void OnPointerDownAction(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        var mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        mousePos.z = 0;
        
        _spawnedBuilding = Instantiate(targetBuildingPrefab, mousePos, Quaternion.identity);
        _spawnedBuilding.TogglePreviewImage(true);
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
        _currentAttachedNode = null;
        _isAttached = false;
    }
    
    private void OnPointerUpAction(PointerEventData eventData)
    {
        if (!_isDragging || _spawnedBuilding == null)
            return;
        
        if (_isAttached && _currentAttachedNode)
        {
            _currentAttachedNode.SetBuilding(_spawnedBuilding);
            _spawnedBuilding.TogglePreviewImage(false);
        }
        else
        {
            Destroy(_spawnedBuilding.gameObject);
        }
        
        _spawnedBuilding = null;
    }
}