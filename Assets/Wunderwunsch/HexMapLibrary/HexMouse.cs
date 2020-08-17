using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wunderwunsch.HexMapLibrary
{
    /// <summary>
    /// Updates every frame with the position of the mouse cursor on the XZ-Plane in different coordinate systems. 
    /// </summary>
    public class HexMouse 
    {
        /// <summary>
        /// Map which is assigned to the mouse - should always be the current visible map in cases where you have multiple maps.
        /// if it is null then it will just skip map wrapping and clamping.
        /// </summary>
        private HexMap hexMap;

        /// <summary>
        /// Indicates whether the cursor is on the map.
        /// </summary>
        public bool CursorIsOnMap { get; private set; }

        ///// <summary>
        ///// Indicates whether the mouse coordinates gets clamped to closest valid position on map.
        ///// Cartesian coordinates are never clamped.
        ///// </summary>
        //public bool ClampToClosestValid { get; private set; }

        /// <summary>
        /// cartesian coordinate without map wrap considered
        /// </summary>
        public Vector3 CartesianCoordInfiniteGrid { get; private set; }
        /// <summary>
        /// cartesian coordinate with map wrap considered
        /// </summary>
        public Vector3 CartesianCoordWrapped { get; private set; }

        /// <summary>
        /// cube coordinate without map wrap
        /// </summary>
        public Vector3Int CubeCoordRaw { get; private set; }
        /// <summary>
        /// cube coordinate with map wrap
        /// </summary>
        public Vector3Int TileCoord { get; private set; }

        /// <summary>
        /// offset coordinate without map wrap
        /// </summary>
        public Vector2Int OffsetCoordInfiniteGrid { get; private set; }
        /// <summary>
        /// offset coordinate with map wrap
        /// </summary>
        public Vector2Int OffsetCoord { get; private set; }

        /// <summary>
        /// closest edge coordinate without map wrap
        /// </summary>
        public Vector3Int ClosestEdgeCoordInfiniteGrid { get; private set; }
        /// <summary>
        /// closest edge coordinate with map wrap
        /// </summary>
        public Vector3Int ClosestEdgeCoord { get; private set; }

        /// <summary>
        /// equals Camera.main.ScreenPointToRay(Input.mousePosition);
        /// </summary>
        public Ray SelectionRay { get; private set; }

        /// <summary>
        /// closest corner coordinate without map wrap
        /// </summary>
        public Vector3Int ClosestCornerCoordInfiniteGrid { get; private set; }

        /// <summary>
        /// closest corner coordinate with map wrap
        /// </summary>
        public Vector3Int ClosestCornerCoord { get; private set; }

        /// <summary>
        /// Call this at start of game or when the map changes to assign the HexMap
        /// </summary>        
        public void Init(HexMap hexMap, bool useMonoBehaviourHelper = true)
        {
            this.hexMap = hexMap;
            if (useMonoBehaviourHelper)
            {
                var go = new GameObject();
                go.name = "HexMapMouseHelper";
                var hexMouseMonoBehaviour = go.AddComponent<HexMouseMonoBehaviour>();
                hexMouseMonoBehaviour.Init(UpdateMousePositionData);
            }
        }

        /// <summary>
        /// updates all the mouse position data
        /// </summary>
        public void UpdateMousePositionData(Vector3 point)
        {
            CursorIsOnMap = false;
            CartesianCoordInfiniteGrid = point;
            CartesianCoordWrapped = CartesianCoordInfiniteGrid;

            CubeCoordRaw = HexConverter.CartesianCoordToTileCoord(CartesianCoordInfiniteGrid);
            TileCoord = CubeCoordRaw;

            OffsetCoordInfiniteGrid = HexConverter.CartesianCoordToOffsetCoord(CartesianCoordInfiniteGrid);
            OffsetCoord = OffsetCoordInfiniteGrid;

            ClosestEdgeCoordInfiniteGrid = HexConverter.CartesianCoordToClosestEdgeCoord(CartesianCoordInfiniteGrid);
            ClosestEdgeCoord = ClosestEdgeCoordInfiniteGrid;

            ClosestCornerCoordInfiniteGrid = HexConverter.CartesianCoordToClosestCornerCoord(CartesianCoordInfiniteGrid);
            ClosestCornerCoord = ClosestCornerCoordInfiniteGrid;

            Vector3 mousePosition = Vector3.zero;
#if ENABLE_INPUT_SYSTEM
            mousePosition = Mouse.current.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            mousePosition = Input.mousePosition
#endif
            SelectionRay = Camera.main.ScreenPointToRay(mousePosition);


            if (hexMap != null)
            {
                if (hexMap.CoordinateWrapper != null)
                {
                    CartesianCoordWrapped = hexMap.CoordinateWrapper.WrapCartesianCoordinate(CartesianCoordInfiniteGrid);
                }

                CursorIsOnMap = hexMap.GetTilePosition.IsInputCoordinateOnMap(CartesianCoordWrapped);

                TileCoord = HexConverter.CartesianCoordToTileCoord(CartesianCoordWrapped);
                OffsetCoord = HexConverter.TileCoordToOffsetTileCoord(TileCoord);
                ClosestEdgeCoord = HexConverter.CartesianCoordToClosestEdgeCoord(CartesianCoordWrapped);
                ClosestCornerCoord = HexConverter.CartesianCoordToClosestCornerCoord(CartesianCoordWrapped);             
            }
        }
    }
}