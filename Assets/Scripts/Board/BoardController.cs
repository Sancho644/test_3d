using System.Collections.Generic;
using Config;
using Core;
using Stack;
using UnityEngine;

namespace Board
{
    public class BoardController : MonoBehaviour
    {
        [Header("Cells on board")]
        [SerializeField] private List<HexCell> cells;
        [SerializeField] private ColorDatabase colorDatabase;
        [SerializeField] private GameController gameController;

        private Dictionary<int, HexCell> _cellMap;
        
        public List<HexCell> Cells { get { return cells; } }
        
        private void Awake()
        {
            _cellMap = new Dictionary<int, HexCell>();

            foreach (var cell in cells)
            {
                _cellMap[cell.Id] = cell;
                cell.Initialize(colorDatabase, gameController);
            }
        }
        
        public bool PlaceStack(HexCell cell, StackView stack)
        {
            if (cell == null || !cell.IsEmpty)
                return false;

            cell.CurrentStacks.Add(stack.Data);

            stack.transform.position = new Vector3(
                cell.transform.position.x,  
                cell.transform.position.y + cell.CellHeight, 
                cell.transform.position.z);

            return true;
        }
        
        public HexCell GetNearestEmptyCell(Vector3 worldPos, float snapRadius = 2f)
        {
            HexCell bestCell = null;
            var minDist = snapRadius;

            foreach (var cell in cells)
            {
                if (!cell.IsEmpty)
                    continue;

                var dist = Vector3.Distance(worldPos, cell.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    bestCell = cell;
                }
            }

            return bestCell;
        }
        
        public void ClearCell(HexCell cell)
        {
            /*if (cell == null || cell.IsEmpty)
                return;

            Destroy(cell.CurrentStacks.gameObject);

            cell.CurrentStack = null;*/
        }
    }
}