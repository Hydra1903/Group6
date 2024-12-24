using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using TMPro;
public class SoitileManager : MonoBehaviour
{
    [Header("TileBase")]
    //public TileBase dirtTile;       // Tile đất bình thường
    public TileBase[] dirtTile = new TileBase[5]; // Khởi tạo mảng với các phần tử cụ thể

    public TileBase dugTile;        // Tile đất đã đào
    public TileBase seededTile;     // Tile đất đã gieo hạt (để đánh dấu trên SoilLayer)
    //public TileBase plantSeedTile;  // Tile hạt giống trên PlantLayer
    public TileBase wateredSoilTile; // Tham chiếu đến Tile đậm hơn cho ô đất đã tưới

    [Header("TileMap")]
    private Tilemap soilTilemap;    // Tilemap cho đất (SoilLayer)
    public Tilemap plantTilemap;   // Tilemap cho cây trồng (PlantLayer)

    [Header("FruitDrop")]
    //public GameObject fruitPrefab; // Gán prefab của quả vào trong Inspector
    //public Transform fruitDropPosition;

    [Header("Fruit Prefabs")]
    public GameObject tomatoFruitPrefab; // Prefab cho quả Tomato
    public GameObject carrotFruitPrefab; // Prefab cho quả Carrot
    public GameObject potatoFruitPrefab; // Prefab cho quả Potato

    [Header("TileBase - Carrot")]
    public TileBase carrotSeedTile;
    public TileBase carrotSproutTile;
    public TileBase carrotYoungPlantTile;
    public TileBase carrotFlowerPlantTile;
    public TileBase carrotFruitTile;

    [Header("TileBase - Tomato")]
    public TileBase tomatoSeedTile;
    public TileBase tomatoSproutTile;
    public TileBase tomatoYoungPlantTile;
    public TileBase tomatoFlowerPlantTile;
    public TileBase tomatoFruitTile;

    [Header("TileBase - Potato")]
    public TileBase potatoSeedTile;
    public TileBase potatoSproutTile;
    public TileBase potatoYoungPlantTile;
    public TileBase potatoFlowerPlantTile;
    public TileBase potatoFruitTile;

    [Header("FruitData")]
    //public TileBase sproutTile, youngPlantTile, fruitTile; // Các Tile cho từng giai đoạn
    private Dictionary<Vector3Int, CropTile> crops = new Dictionary<Vector3Int, CropTile>(); // Lưu trữ cây trồng trên Tilemap

    // Seed Selection Variables
    private string selectedSeed = null; // Currently selected seed type
    private Dictionary<string, SeedData> seedData; // Dictionary to store seed properties

    //public InventoryManager playerInventory;
    public HighlightController highlightController;
    public Transform player;            // Tham chiếu đối tượng Player
    public Player toolCheck;
    //public Animator anim;

    // Kích thước của một tile trong TileMap (giả sử kích thước tile là 1x1)
    public float tileSize = 1f;
    // Chỉnh sửa offset (có thể là 1 ô về một hướng nào đó)
    // Cập nhật offset khi Player di chuyển
    private Vector3Int offset = Vector3Int.zero;

    private Vector3 playerPosition = Vector3.zero;

    [Header("Weather/ WateredSoil")]
    public WeatherManager weatherManager;
    //private float timeSinceRainStopped;
    //private float dryOutTime = 30f; // Thời gian để đất khô (đơn vị: giây)
    private TileBase[,] originalTileStates;
    // Biến thời gian để theo dõi thời gian sau khi mưa dừng
    private float timeSinceRainStopped = 0f;
    private bool isRaining = false;  // Theo dõi trạng thái mưa
    private float timeSinceWatered = 0f;
    private bool isWatered = false;

    private Vector3Int currentGridPos;


    public GameObject wateredIconPrefab; // Prefab của icon tưới nước
    private Dictionary<Vector3Int, GameObject> activeIcons = new Dictionary<Vector3Int, GameObject>(); // Lưu các icon đang hiển thị

    [SerializeField] private GameObject cropInfoPanel;
    [SerializeField] private TMPro.TextMeshProUGUI cropInfoText; // Text hiển thị thông tin
    private Vector3Int currentCellPosition; // Lưu vị trí cây đang hiển thị

    [SerializeField] private GameObject bloomGrowthEffectPrefab; // Kéo thả Prefab hiệu ứng vào đây                                                             
    private Dictionary<Vector3Int, GameObject> cropEffects = new Dictionary<Vector3Int, GameObject>(); // Dictionary để lưu hiệu ứng hương thơm tương ứng với vị trí cây

    void Start()
    {
        soilTilemap = GameObject.Find("SoilLayer").GetComponent<Tilemap>();
        plantTilemap = GameObject.Find("PlantLayer").GetComponent<Tilemap>();

        if (soilTilemap == null || plantTilemap == null)
        {
            Debug.LogError("Không tìm thấy Tilemap cho SoilLayer hoặc PlantLayer!");
        }

        // Khởi tạo dữ liệu hạt giống
        seedData = new Dictionary<string, SeedData>
        {
            { "Carrot Seed", new SeedData(carrotSeedTile,carrotSproutTile, carrotYoungPlantTile, carrotFlowerPlantTile ,carrotFruitTile,1) },
            { "Tomato Seed", new SeedData(tomatoSeedTile,tomatoSproutTile, tomatoYoungPlantTile, tomatoFlowerPlantTile ,tomatoFruitTile,2) },
            { "Potato Seed", new SeedData(potatoSeedTile,potatoSproutTile, potatoYoungPlantTile, potatoFlowerPlantTile ,potatoFruitTile,1) },
        };
    }


    void Update()
    {
        //WateredTile duration
        WateredDuration();

        UpdateCrops(Time.deltaTime);

        if (toolCheck.isUsingTool)
        {
            // Sử dụng currentCell từ HighlightController
            Vector3Int currentGridPos = highlightController.currentCell;

            if (Input.GetKeyDown(KeyCode.F) && Player.instance != null)
            {
                // Kiểm tra xem người chơi có thể đào đất
                if (Player.instance.CanDig())
                {
                    TileBase currentTile = soilTilemap.GetTile(currentGridPos);
                    if (System.Array.Exists(dirtTile, tile => tile == currentTile))
                    {
                        QuestManager.instance.UpdateQuestProgress(ActionType.DigSoil);
                        Dig(currentGridPos);
                    }
                }

                // Kiểm tra xem người chơi có thể gieo hạt
                else if (Player.instance.CanPlantSeeds())
                {
                    if (selectedSeed == null)
                    {
                        Debug.Log("Vui lòng chọn hạt giống trước khi trồng.");
                        return;
                    }

                    TileBase currentTile = soilTilemap.GetTile(currentGridPos);
                    if (currentTile == dugTile)
                    {
                        PlantSeed(currentGridPos);
                    }
                    else
                    {
                        Debug.Log("Ô đất chưa được đào");
                    }
                }

                // Kiểm tra xem người chơi có thể tưới nước
                else if (Player.instance.CanWatering())
                {
                    TileBase currentTile = soilTilemap.GetTile(currentGridPos);
                    if (currentTile == seededTile)
                    {
                        WaterCrop(currentGridPos);
                    }
                    else
                    {
                        Debug.Log("Ô đất chưa có cây");
                    }
                }

                // Kiểm tra xem người chơi có thể thu hoạch
                else if (Player.instance.CanHarvest())
                {
                    TileBase currentTile = plantTilemap.GetTile(currentGridPos);
                    if (currentTile != null && crops.ContainsKey(currentGridPos) && crops[currentGridPos].isHarvestable)
                    {
                        Harvest(currentGridPos);
                    }
                    else
                    {
                        Debug.Log("Cây chưa thể thu hoạch hoặc không có cây để thu hoạch.");
                    }
                }
            }
        }

        // Weather Effected
        if (weatherManager.isRaining)
        {
            isRaining = true;
            timeSinceRainStopped = 0f;


            // Lặp qua tất cả các vị trí trong tilemap (trong phạm vi giới hạn của tilemap)
            foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
            {
                // Kiểm tra nếu ô đất hiện tại có tồn tại một Tile
                TileBase currentTile = soilTilemap.GetTile(cellPosition);
                if (currentTile == null) continue; // Bỏ qua nếu không có Tile tại vị trí này

                // Kiểm tra nếu ô đất là dugTile hoặc seededTile
                if (currentTile == dugTile || currentTile == seededTile)
                {
                    // Cập nhật trạng thái watered cho cây trồng (nếu có) tại vị trí này
                    if (crops.ContainsKey(cellPosition))
                    {
                        CropTile crop = crops[cellPosition];
                        crop.isWatered = true;
                    }

                    // Đặt ô đất thành wateredSoilTile
                    soilTilemap.SetTile(cellPosition, wateredSoilTile);
                }
            }
        }
        else
        {
            if (isRaining)
            {
                // Trời đã ngừng mưa, bắt đầu đếm thời gian
                timeSinceRainStopped += Time.deltaTime;

                // Sau 30 giây (30 phút trong game), trả các ô về trạng thái ban đầu
                if (timeSinceRainStopped >= 30f)
                {
                    isRaining = false; // Đặt lại trạng thái mưa

                    foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
                    {
                        TileBase currentTile = soilTilemap.GetTile(cellPosition);
                        if (currentTile == null) continue;

                        // Trả các ô về trạng thái dugTile hoặc seededTile
                        if (currentTile == wateredSoilTile)
                        {
                            if (crops.ContainsKey(cellPosition) && crops[cellPosition].isPlanted)
                            {
                                soilTilemap.SetTile(cellPosition, seededTile); // Trả về seededTile nếu có cây trồng
                            }
                            else
                            {
                                soilTilemap.SetTile(cellPosition, dugTile); // Trả về dugTile nếu không có cây trồng
                            }
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = plantTilemap.WorldToCell(mouseWorldPos);

            if (crops.ContainsKey(cellPosition))
            {
                ShowCropInfo(cellPosition);
            }
            else
            {
                cropInfoPanel.SetActive(false); // Ẩn Panel nếu click ngoài cây
            }
        }
    }

    
    private void PlayerPosition()
    {
        // Dùng các phím mũi tên hoặc WASD để thay đổi offset
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            playerPosition = new Vector3(0, -0.3f, 0); //+ player.position;  // Di chuyển lên   
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            playerPosition = new Vector3(0, -0.27f, 0); //+ player.position;  // Di chuyển xuống   
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            playerPosition = new Vector3(-0.5f, -0.25f, 0); //+ player.position;  // Di chuyển phải  
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            playerPosition = new Vector3(0.5f, -0.25f, 0);// + player.position;  // Di chuyển trái  
        }
    }

    private Vector3Int GetGridPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = soilTilemap.WorldToCell(mouseWorldPos);
        return gridPosition;
    }

    void Dig(Vector3Int gridPos)
    {
        soilTilemap.SetTile(gridPos, dugTile);
        Debug.Log("Đã đào đất tại: " + gridPos);
    }

    public void SelectSeed(string seedName)
    {

        if (seedData.ContainsKey(seedName))
        {
            selectedSeed = seedName;
            Debug.Log("Đã chọn hạt giống: " + seedName);
        }
        else
        {
            Debug.Log("Hạt giống không hợp lệ!");
        }
    }

    // Thêm cây vào Tilemap khi gieo hạt
    public void PlantSeed(Vector3Int position)
    {
        if (selectedSeed == null || !seedData.ContainsKey(selectedSeed)) return;

        // Kiểm tra trong kho xem có đủ số lượng hạt giống không
        if (!InventoryManager.instance.HasItem(selectedSeed))
        {
            Debug.Log("Không có đủ hạt giống trong kho để gieo!");
            return;
        }
        // Kiểm tra nếu vị trí đã có cây để không bị thay thế
        if (crops.ContainsKey(position))
        {
            Debug.Log("Đã có cây trồng ở vị trí này. Không thể gieo hạt mới.");
            return;
        }
        SeedData seed = seedData[selectedSeed];

        // Giảm số lượng hạt giống trong kho
        InventoryManager.instance.RemoveItem(selectedSeed, 1);

        CropTile newCrop = new CropTile()
        {
            growthStage = 0,
            //hoursGrowth = 0f,

            isPlanted = true,
            isWatered = false,
            isHarvestable = false,
            seedName = selectedSeed // Lưu tên hạt giống để sử dụng khi cập nhật
        };
        crops[position] = newCrop;
        plantTilemap.SetTile(position, seed.seedTile); // Bắt đầu với Tile hạt giống


        // Cập nhật tiến trình nhiệm vụ
        QuestManager.instance.UpdateQuestProgress(ActionType.SowSeeds, selectedSeed);
    }


    // Phương thức tưới nước cho cây
    public void WaterCrop(Vector3Int position)
    {
        if (crops.ContainsKey(position))
        {
            crops[position].isWatered = true;
            CropTile crop = crops[position];
            crop.hoursGrowth = 0f;
            isWatered = true;
            //crops[position].wateredTime = 0.5f; // Đặt thời gian tưới 
            timeSinceWatered = 0f;
            soilTilemap.SetTile(position, wateredSoilTile);
            Debug.Log($"Watered time set to: {timeSinceWatered}");

            // Cập nhật tiến trình nhiệm vụ
            QuestManager.instance.UpdateQuestProgress(ActionType.WaterSoil);
        }

    }
    public void WateredDuration()
    {
        if (isWatered)
        {
            // Trời đã ngừng mưa, bắt đầu đếm thời gian
            timeSinceWatered += Time.deltaTime;

            // Sau 30 giây (30 phút trong game), trả các ô về trạng thái ban đầu
            if (timeSinceWatered >= 30f)
            {
                isWatered = false; // Đặt lại trạng thái mưa

                foreach (Vector3Int cellPosition in soilTilemap.cellBounds.allPositionsWithin)
                {
                    TileBase currentTile = soilTilemap.GetTile(cellPosition);
                    if (currentTile == null) continue;

                    // Trả các ô về trạng thái dugTile hoặc seededTile
                    if (currentTile == wateredSoilTile)
                    {
                        if (crops.ContainsKey(cellPosition))
                        {
                            soilTilemap.SetTile(cellPosition, seededTile); // Trả về seededTile nếu có cây trồng
                        }
                    }
                }
            }
        }
    }

    // Hàm Harvest
    public void Harvest(Vector3Int gridPos)
    {
        if (!crops.ContainsKey(gridPos) || !crops[gridPos].isHarvestable)
        {
            Debug.Log("Cây chưa thể thu hoạch hoặc không có cây để thu hoạch.");
            return;
        }

        CropTile crop = crops[gridPos];
        GameObject fruitPrefab = null;

        // Chọn đúng Prefab của trái cây dựa trên loại hạt giống
        if (crop.seedName == "Tomato Seed")
        {
            fruitPrefab = tomatoFruitPrefab;
        }
        else if (crop.seedName == "Carrot Seed")
        {
            fruitPrefab = carrotFruitPrefab;
        }
        else if (crop.seedName == "Potato Seed")
        {
            fruitPrefab = potatoFruitPrefab;
        }

        if (fruitPrefab != null)
        {
            // Instantiate trái cây tại vị trí của ô đất
            Vector3 dropPosition = soilTilemap.CellToWorld(gridPos) + new Vector3(0.7f, 0.3f, 0);
            Instantiate(fruitPrefab, dropPosition, Quaternion.identity);
            Debug.Log($"+1 {crop.seedName} Fruit");

            // Cập nhật tiến trình nhiệm vụ
            QuestManager.instance.UpdateQuestProgress(ActionType.HarvestCrop, crop.seedName);
        }

        // Đặt lại tile trên plantTilemap về None để xóa cây
        plantTilemap.SetTile(gridPos, null);

        // Đặt lại tile trên soilTilemap thành dugTile để biểu thị đất đã đào
        soilTilemap.SetTile(gridPos, dugTile);
        //wateredIconTilemap.SetTile(gridPos, null); // Xóa icon
        RemoveWateredIcon(gridPos); // Xóa icon ở giai đoạn cuối

        // Hủy hiệu ứng hương thơm nếu tồn tại
        if (cropEffects.ContainsKey(gridPos))
        {
            Destroy(cropEffects[gridPos]); // Hủy GameObject hiệu ứng
            cropEffects.Remove(gridPos);  // Xóa khỏi Dictionary
        }

        // Xóa cây trồng khỏi danh sách crops
        crops.Remove(gridPos);

        Debug.Log("Thu hoạch thành công! Đất đã được trả về trạng thái đào.");
    }
    private Vector3 GetAdjustedIconPosition(Vector3Int cropPosition)
    {
        // Điều chỉnh vị trí icon thấp hơn 0.5 - 0.7 đơn vị
        Vector3 worldPosition = plantTilemap.CellToWorld(cropPosition);
        return worldPosition + new Vector3(0.2f, 0.7f, 0); //  hạ icon
    }

    public void UpdateCrops(float deltaTime)
    {
        List<Vector3Int> positions = new List<Vector3Int>(crops.Keys);
        foreach (var position in positions)
        {
            CropTile crop = crops[position];
            SeedData seed = seedData[crop.seedName];

            //Vector3Int iconPosition = GetIconPosition(position); // Vị trí icon tưới nước
            Vector3 iconWorldPosition = GetAdjustedIconPosition(position); // Vị trí icon sau khi điều chỉnh
            // Vị trí hiệu ứng hương thơm
            Vector3 worldPosition = plantTilemap.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0);

            // Nếu cây được tưới, cho phép tăng thời gian phát triển
            if (crop.isWatered)
            {
                crop.hoursGrowth += deltaTime;

                // Kiểm tra điều kiện chuyển giai đoạn phát triển
                if (crop.growthStage == 0 && crop.hoursGrowth >= 20f)
                {

                    crop.growthStage = 1;
                    crop.hoursGrowth = 0;
                    // Hiệu ứng hương thơm
                    GameObject effect = Instantiate(bloomGrowthEffectPrefab, worldPosition, Quaternion.identity);

                    // Hủy hiệu ứng sau khi chạy xong (nếu cần)
                    Destroy(effect, 5f); // Thời gian phù hợp với Duration của Particle
                    // Gọi hiệu ứng fade-in
                    StartFadeEffect(position, seed.sproutTile);
                    plantTilemap.SetTile(position, seed.sproutTile);
                    crop.isWatered = false; // Cần tưới lại cho giai đoạn mới

                    ShowWateredIcon(position, iconWorldPosition); // Hiển thị icon mới

                }
                else if (crop.growthStage == 1 && crop.hoursGrowth >= 30f)
                {

                    crop.growthStage = 2;
                    crop.hoursGrowth = 0;
                    // Hiệu ứng hương thơm
                    GameObject effect = Instantiate(bloomGrowthEffectPrefab, worldPosition, Quaternion.identity);

                    // Hủy hiệu ứng sau khi chạy xong (nếu cần)
                    Destroy(effect, 5f); // Thời gian phù hợp với Duration của Particle
                    // Gọi hiệu ứng fade-in
                    StartFadeEffect(position, seed.youngPlantTile);
                    plantTilemap.SetTile(position, seed.youngPlantTile);
                    crop.isWatered = false; // Cần tưới lại cho giai đoạn mới

                    RemoveWateredIcon(position); // Xóa icon cũ
                    ShowWateredIcon(position, iconWorldPosition); // Hiển thị icon mới
                }
                else if (crop.growthStage == 2 && crop.hoursGrowth >= 30f)
                {

                    crop.growthStage = 3;
                    crop.hoursGrowth = 0;
                    // Hiệu ứng hương thơm
                    GameObject effect = Instantiate(bloomGrowthEffectPrefab, worldPosition, Quaternion.identity);

                    // Hủy hiệu ứng sau khi chạy xong (nếu cần)
                    Destroy(effect, 5f); // Thời gian phù hợp với Duration của Particle
                    // Gọi hiệu ứng fade-in
                    StartFadeEffect(position, seed.flowerPlantTile);
                    plantTilemap.SetTile(position, seed.flowerPlantTile);
                    crop.isWatered = false; // Cần tưới lại cho giai đoạn mới

                    RemoveWateredIcon(position); // Xóa icon cũ
                    ShowWateredIcon(position, iconWorldPosition); // Hiển thị icon mới
                }
                else if (crop.growthStage == 3 && crop.hoursGrowth >= 40f)
                {

                    crop.growthStage = 4;
                    crop.hoursGrowth = 0;
                    // Hiệu ứng hương thơm
                    GameObject effect = Instantiate(bloomGrowthEffectPrefab, worldPosition, Quaternion.identity);
                    // Lưu hiệu ứng vào Dictionary
                    cropEffects[position] = effect;
                    // Gọi hiệu ứng fade-in
                    StartFadeEffect(position, seed.fruitTile);
                    plantTilemap.SetTile(position, seed.fruitTile);
                    crop.isHarvestable = true;
                    crop.isWatered = false; // Đặt lại trạng thái tưới

                    // Hủy hiệu ứng sau khi chạy xong (nếu cần)
                    //Destroy(effect, 5f); // Thời gian phù hợp với Duration của Particle

                    RemoveWateredIcon(position); // Xóa icon ở giai đoạn cuối

                }
            }
            else
            {
                // Nếu chưa được tưới, hiển thị icon nếu chưa có
                if (!crop.isHarvestable && !activeIcons.ContainsKey(position))
                {
                    ShowWateredIcon(position, iconWorldPosition);
                }
            }

            // Xóa icon sau khi tưới
            if (crop.isWatered)
            {
                RemoveWateredIcon(position); // Xóa icon ở giai đoạn cuối
            }
            // Khi ở giai đoạn thu hoạch loại bỏ icon
            if (crop.isHarvestable && crop.growthStage == 4)
            {
                RemoveWateredIcon(position); // Xóa icon ở giai đoạn cuối
            }
        }
    }
    private void ShowWateredIcon(Vector3Int cropPosition, Vector3 worldPosition)
    {
        if (!activeIcons.ContainsKey(cropPosition))
        {
            GameObject icon = Instantiate(wateredIconPrefab, worldPosition, Quaternion.identity);
            activeIcons[cropPosition] = icon; // Lưu icon theo vị trí cây
        }
    }

    private void RemoveWateredIcon(Vector3Int cropPosition)
    {
        if (activeIcons.ContainsKey(cropPosition))
        {
            Destroy(activeIcons[cropPosition]); // Xóa icon khỏi scene
            activeIcons.Remove(cropPosition);  // Xóa khỏi danh sách
        }
    }


    private IEnumerator FadeFromDarkToBright(SpriteRenderer spriteRenderer, float duration, Vector3Int position, TileBase tile)
    {
        float elapsed = 0f;

        // Bắt đầu từ màu tối (đen hoặc xám)
        Color color = spriteRenderer.color;
        color.a = 1f; // Đảm bảo không trong suốt
        color.r = 0.2f; // Màu xám (giá trị RGB thấp)
        color.g = 0.2f;
        color.b = 0.2f;
        spriteRenderer.color = color;

        // Fade-in từ tối đến sáng
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Tăng độ sáng (RGB dần về 1) nhưng giữ alpha ở mức 1
            color.r = Mathf.Lerp(0.2f, 1f, progress);
            color.g = Mathf.Lerp(0.2f, 1f, progress);
            color.b = Mathf.Lerp(0.2f, 1f, progress);
            spriteRenderer.color = color;

            yield return null;
        }

        // Đặt tile mới sau khi hiệu ứng hoàn tất
        plantTilemap.SetTile(position, tile);

        // Xóa GameObject hiệu ứng sau khi hoàn thành
        Destroy(spriteRenderer.gameObject);
    }



    private void StartFadeEffect(Vector3Int position, TileBase tile)
    {
        // Tạo GameObject tạm thời cho hiệu ứng
        GameObject fadeObject = new GameObject("FadeEffect");
        SpriteRenderer spriteRenderer = fadeObject.AddComponent<SpriteRenderer>();

        // Lấy sprite từ tile
        spriteRenderer.sprite = (tile as UnityEngine.Tilemaps.Tile)?.sprite;
        spriteRenderer.sortingLayerName = "Platform";
        spriteRenderer.sortingOrder = 0;

        // Đặt vị trí sprite vào giữa tile
        Vector3 worldPosition = plantTilemap.CellToWorld(position);
        Vector3 tileCenterOffset = new Vector3(0.5f, 0.523f, 0);
        fadeObject.transform.position = worldPosition + tileCenterOffset;

        // Bắt đầu hiệu ứng fade-in (màu tối sáng dần)
        StartCoroutine(FadeFromDarkToBright(spriteRenderer, 5f, position, tile));
    }

    private void DisplayCropInfoUI(string info, Vector3Int cellPosition)
    {
        // Hiển thị thông tin lên Text
        cropInfoText.text = info;

        // Lấy vị trí world position của tile
        Vector3 worldPosition = plantTilemap.CellToWorld(cellPosition);

        // Thêm offset để Panel xuất hiện ngay trên đầu cây
        Vector3 offset = new Vector3(0.5f, 1.5f, 0); // Tùy chỉnh giá trị này nếu cần
        Vector3 panelWorldPosition = worldPosition + offset;

        // Đặt vị trí Panel (vì Canvas là World Space)
        cropInfoPanel.transform.position = panelWorldPosition;

        // Bật Panel
        cropInfoPanel.SetActive(true);
    }

    private void ShowCropInfo(Vector3Int cellPosition)
    {
        // Kiểm tra nếu đang hiển thị thông tin của cây khác, tắt Panel
        if (cropInfoPanel.activeSelf && cellPosition != currentCellPosition)
        {
            StopAllCoroutines(); // Dừng Coroutine hiện tại
            cropInfoPanel.SetActive(false);
        }

        currentCellPosition = cellPosition;

        CropTile crop = crops[cellPosition];

        // Lấy dữ liệu giai đoạn và thời gian còn lại
        int growthStage = crop.growthStage;
        float hoursGrowth = crop.hoursGrowth;
        float requiredTime = GetRequiredTimeForStage(growthStage);
        float remainingTime = Mathf.Max(0, requiredTime - hoursGrowth);

        // Hiển thị thông tin ban đầu lên UI
        DisplayCropInfoUI($"Stage: {growthStage}\nRemaining Time: {FormatTime(remainingTime)}", cellPosition);

        // Bắt đầu cập nhật đếm ngược
        StartCoroutine(UpdateCountdown(crop, cellPosition));
    }
    private IEnumerator UpdateCountdown(CropTile crop, Vector3Int cellPosition)
    {
        float requiredTime = GetRequiredTimeForStage(crop.growthStage);

        while (crop.hoursGrowth < requiredTime)
        {
            // Tính thời gian còn lại
            float remainingTime = Mathf.Max(0, requiredTime - crop.hoursGrowth);

            // Cập nhật thông tin đếm ngược
            cropInfoText.text = $"Stage: {crop.growthStage}\nRemaining Time: {FormatTime(remainingTime)}";

            // Đợi 0.5 giây trước khi cập nhật lại
            yield return new WaitForSeconds(0.5f);
        }

        // Khi hoàn tất giai đoạn, ẩn Panel
        cropInfoPanel.SetActive(false);
    }

    private float GetRequiredTimeForStage(int stage)
    {
        switch (stage)
        {
            case 0: return 20f; // Giai đoạn 0 cần 20 phút
            case 1: return 30f; // Giai đoạn 1 cần 30 phút
            case 2: return 30f; // Giai đoạn 2 cần 30 phút
            case 3: return 40f; // Giai đoạn 2 cần 40 phút

            default: return 0f; // Không yêu cầu thời gian ở giai đoạn cuối
        }
    }
    private string FormatTime(float hours)
    {
        int h = Mathf.FloorToInt(hours);
        int m = Mathf.FloorToInt((hours - h) * 60);
        return $"{h:D2}:{m:D2}";
    }
}
public class SeedData
{
    public TileBase seedTile;
    public TileBase sproutTile;
    public TileBase youngPlantTile;
    public TileBase flowerPlantTile;
    public TileBase fruitTile;
    public int timeGrowth;

    public SeedData(TileBase seedTile, TileBase sproutTile, TileBase youngPlantTile, TileBase flowerPlantTile, TileBase fruitTile, int timeGrowth)
    {
        this.seedTile = seedTile;
        this.sproutTile = sproutTile;
        this.youngPlantTile = youngPlantTile;
        this.flowerPlantTile = flowerPlantTile;
        this.fruitTile = fruitTile;
        this.timeGrowth = timeGrowth;
        this.flowerPlantTile = flowerPlantTile;
    }
}








