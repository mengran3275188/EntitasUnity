using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Spatial;
using Util;

namespace WindowsFormsApplication1
{
    public enum FunctionType
    {
        kMoveObj = 0,
        kGetCell,
        kSetStaticBlock,
        kHitPoint,
        kAddObj,
        kQueryObj,
    };

    public enum KeyHit
    {
        W = 1,
        A = 2,
        S = 4,
        D = 8,
    }

    public partial class Form1 : Form
    {
        private Pen line_pen = new Pen(Color.Red, 1.0f);
        private Pen path_pen = new Pen(Color.Black, 1.0f);
        private Pen obj_pen = new Pen(Color.Blue, 1.0f);
        private Pen point_pen = new Pen(Color.Gold, 1.0f);
        private Pen space_obj_pen = new Pen(Color.DarkBlue, 1.0f);

        private Pen kdtree_pen = new Pen(Color.DarkOliveGreen, 3.0f);
        private Pen prkdtree_pen = new Pen(Color.DarkViolet, 2.0f);
        private Pen kdgeotree_pen = new Pen(Color.Black, 3.0f);

        private Pen pen = new Pen(Color.FromArgb(100, Color.Red), 1.0f);
        private Pen grid_pen = new Pen(Color.FromArgb(100, Color.White), 1.0f);
        private Pen marker_pen = new Pen(Color.LightYellow, 1.0f);
        private Pen triangle_pen = new Pen(Color.Yellow, 1.0f);

        private SolidBrush brush = new SolidBrush(Color.FromArgb(32, Color.Red));
        private SolidBrush point_brush = new SolidBrush(Color.Black);
        private SolidBrush obj_brush = new SolidBrush(Color.DarkCyan);
        private SolidBrush space_obj_brush = new SolidBrush(Color.DarkGray);
        private SolidBrush triangle_brush = new SolidBrush(Color.LightBlue);
        private SolidBrush path_brush = new SolidBrush(Color.DarkRed);

        private SolidBrush marker_brush = new SolidBrush(Color.LightYellow);
        private SolidBrush kdtree_select_brush = new SolidBrush(Color.LightGreen);
        private SolidBrush prkdtree_select_brush = new SolidBrush(Color.Yellow);

        private Timer mFormsTimer;
        private BufferedGraphicsContext context_;
        private BufferedGraphics graphics_;
        private BufferedGraphics obstacle_graphics_;
        private Image back_image_;

        private string map_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.map";
        private string path_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.path";
        private string obstacle_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.tmx";
        private string obs_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.obs";
        private string nav_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.obs";
        private string map_patch_file_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.map.patch";
        private string map_image_ = "F:/DashFire/Public/Resource/Public/Scenes/MapData_1012/Scene.jpg";
        private float map_actual_width_ = 96;
        private float map_actual_height_;
        private float cell_actual_width_ = 0.5f;
        private float map_width_;
        private float map_height_;
        private float scale;
        private float cell_width_;
        private Point line_start_;
        private Point line_end_;
        private TiledMapParser tiled_map_parser_ = null;
        private NavmeshMapParser nav_map_parser_ = null;
        private MapPatchParser map_patch_parser_ = null;
        private CellManager cell_manager_ = null;
        private Spatial.SpatialSystem spatial_system_ = null;
        private TestSpaceObject control_obj_ = new TestSpaceObject(1);
        private List<TestSpaceObject> space_objs_ = new List<TestSpaceObject>();
        private List<TestSpaceObject> selected_objs_ = new List<TestSpaceObject>();
        private List<TestSpaceObject> selected_objs2_ = new List<TestSpaceObject>();
        private uint next_space_objid_ = 2;
        private PrKdTree prkdtree_ = null;
        private KdObjectTree kdtree_ = new KdObjectTree();

        private TreeCacheFinder tree_cache_finder_ = new TreeCacheFinder();
        private JumpPointFinder jump_point_finder_ = null;

        private IList<TriangleNode> triangles = null;

        private List<CellPos> hit_points_ = new List<CellPos>();
        private List<Vector3> found_path_ = null;
        private FunctionType left_button_function_ = FunctionType.kMoveObj;

        private bool is_mouse_down_;
        private long last_tick_time_;

        private KeyHit key_hit_;
        private float last_movedir_;
        private int last_adjust_;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mFormsTimer = new System.Windows.Forms.Timer();
            mFormsTimer.Interval = 50;
            mFormsTimer.Tick += new System.EventHandler(this.TimerEventProcessor);
            mFormsTimer.Start();

            context_ = BufferedGraphicsManager.Current;
            last_tick_time_ = TimeUtility.Instance.GetElapsedTimeUs() / 1000;
            last_movedir_ = 0;
            last_adjust_ = 0;

            operation.SelectedIndex = 0;

            LogUtil.OnOutput += (Log_Type type, string msg) =>
            {
                Console.WriteLine("Log {0}:{1}", type, msg);
            };

            FileReaderProxy.RegisterReadFileHandler((string filePath) =>
            {
                byte[] buffer = null;
                try
                {
                    buffer = File.ReadAllBytes(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
                    return null;
                }
                return buffer;
            });
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsValid())
                return;
            switch (e.KeyCode)
            {
                case Keys.A:
                    key_hit_ |= KeyHit.A;
                    break;
                case Keys.S:
                    key_hit_ |= KeyHit.W;
                    break;
                case Keys.D:
                    key_hit_ |= KeyHit.D;
                    break;
                case Keys.W:
                    key_hit_ |= KeyHit.S;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (!IsValid())
                return;
            switch (e.KeyCode)
            {
                case Keys.A:
                    key_hit_ &= (~KeyHit.A);
                    break;
                case Keys.S:
                    key_hit_ &= (~KeyHit.W);
                    break;
                case Keys.D:
                    key_hit_ &= (~KeyHit.D);
                    break;
                case Keys.W:
                    key_hit_ &= (~KeyHit.S);
                    break;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!IsValid())
                return;
            line_end_ = new Point(e.X, e.Y);
            CellPos end = new CellPos();
            cell_manager_.GetCell(new Vector3(e.X, 0, map_height_ - e.Y), out end.row, out end.col);
            CellPos start = new CellPos();
            cell_manager_.GetCell(new Vector3(line_start_.X, 0, map_height_ - line_start_.Y), out start.row, out start.col);

            if (e.Button == MouseButtons.Left)
            {
                switch (left_button_function_)
                {
                    case FunctionType.kSetStaticBlock:
                        {
                            int row = end.row;
                            int col = end.col;
                            byte obstacle = byte.Parse(obstacleType.Text);
                            byte oldObstacle = cell_manager_.GetCellStatus(row, col);
                            cell_manager_.SetCellStatus(row, col, obstacle);

                            if (map_patch_parser_.Exist(row, col))
                            {
                                map_patch_parser_.Update(row, col, obstacle);
                            }
                            else
                            {
                                map_patch_parser_.Update(row, col, obstacle, oldObstacle);
                            }

                            UpdateObstacleGraph();
                        }
                        break;
                    case FunctionType.kHitPoint:
                        {
                            long stTime = TimeUtility.Instance.GetElapsedTimeUs();
                            List<CellPos> result = cell_manager_.GetCellsCrossByLine(new Vector3(line_start_.X, 0, map_height_ - line_start_.Y),
                                                                       new Vector3(line_end_.X, 0, map_height_ - line_end_.Y));
                            long edTime = TimeUtility.Instance.GetElapsedTimeUs();
                            this.Text = "pos(" + line_start_.X + "," + line_start_.Y + "->" + line_end_.X + "," + line_end_.Y + ") GetCellsCrossByLine consume " + (edTime - stTime) + "us";
                            hit_points_.Clear();
                            foreach (CellPos pos in result)
                            {
                                hit_points_.Add(pos);
                            }
                        }
                        break;
                    case FunctionType.kGetCell:
                        {
                            byte obstacle = cell_manager_.GetCellStatus(end.row, end.col);
                            obstacleType.Text = obstacle.ToString();
                            this.Text = "pos(" + e.X + "," + e.Y + ") cell(" + end.row + "," + end.col + ") obstacle:" + obstacle;
                        }
                        break;
                    case FunctionType.kAddObj:
                        {
                            TestSpaceObject obj = new TestSpaceObject(next_space_objid_);
                            ++next_space_objid_;
                            obj.SetPosition(new Vector3(e.X, 0, e.Y));
                            space_objs_.Add(obj);

                            prkdtree_.Clear();
                            kdtree_.Clear();
                            int objCt = space_objs_.Count;
                            if (objCt > 0)
                            {
                                ISpaceObject[] temp = new ISpaceObject[objCt];
                                for (int i = 0; i < objCt; ++i)
                                {
                                    temp[i] = space_objs_[i];
                                }
                                long stTime1 = TimeUtility.Instance.GetElapsedTimeUs();
                                prkdtree_.Build(temp);
                                long edTime1 = TimeUtility.Instance.GetElapsedTimeUs();

                                long stTime2 = TimeUtility.Instance.GetElapsedTimeUs();
                                kdtree_.Build(temp);
                                long edTime2 = TimeUtility.Instance.GetElapsedTimeUs();

                                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                                stopWatch.Start();
                                cell_manager_.ClearDynamic();
                                foreach(ISpaceObject spaceObj in space_objs_)
                                {
                                    List<CellPos> cells = cell_manager_.GetCellsInCircle(Transform(spaceObj.GetPosition()), 20);
                                    foreach(CellPos cellpos in cells)
                                    {
                                        cell_manager_.SetCellStatus(cellpos.row, cellpos.col, BlockType.DYNAMIC_BLOCK);
                                    }
                                }
                                stopWatch.Stop();


                                this.Text = "obj num " + objCt + " prkdtree consume " + (edTime1 - stTime1) + "us kdtree consume " + (edTime2 - stTime1) + "us dynmic consume " + (stopWatch.ElapsedTicks) + "ticks";
                            }
                        }
                        break;
                    case FunctionType.kQueryObj:
                        {
                            Vector3 pos = new Vector3(e.X, 0, e.Y);
                            int objCt = space_objs_.Count;
                            if (objCt > 0)
                            {
                                float radius = float.Parse(queryRadius.Text);
                                long stTime1 = TimeUtility.Instance.GetElapsedTimeUs();
                                selected_objs_.Clear();
                                prkdtree_.Query(pos, radius * scale, (float distSqr, PrKdTreeObject treeObj) =>
                                {
                                    selected_objs_.Add(treeObj.SpaceObject as TestSpaceObject);
                                });
                                long edTime1 = TimeUtility.Instance.GetElapsedTimeUs();

                                long stTime2 = TimeUtility.Instance.GetElapsedTimeUs();
                                selected_objs2_.Clear();
                                kdtree_.Query(pos, radius * scale, (float distSqr, KdTreeObject treeObj) =>
                                {
                                    selected_objs2_.Add(treeObj.SpaceObject as TestSpaceObject);
                                });
                                long edTime2 = TimeUtility.Instance.GetElapsedTimeUs();

                                this.Text = "obj num " + objCt + " query prkdtree " + selected_objs_.Count + " consume " + (edTime1 - stTime1) + "us query kdtree " + selected_objs2_.Count + " consume " + (edTime2 - stTime2) + "us";
                            }
                        }
                        break;
                }
            }
            else
            {
                if (e.Button == MouseButtons.Right && left_button_function_ == FunctionType.kSetStaticBlock)
                {
                    int row, col;
                    cell_manager_.GetCell(new Vector3(e.X, 0, e.Y), out row, out col);
                    byte oldObstacle = cell_manager_.GetCellStatus(row, col);
                    byte obstacle = BlockType.NOT_BLOCK;
                    cell_manager_.SetCellStatus(end.row, end.col, obstacle);

                    if (map_patch_parser_.Exist(row, col))
                    {
                        map_patch_parser_.Update(row, col, obstacle);
                    }
                    else
                    {
                        map_patch_parser_.Update(row, col, obstacle, oldObstacle);
                    }

                    UpdateObstacleGraph();
                }
                else
                {
                    long stTime = TimeUtility.Instance.GetElapsedTimeUs();
                    found_path_ = spatial_system_.FindPathWithCellMap(new Vector3(line_start_.X, 0, map_height_ - line_start_.Y), new Vector3(line_end_.X, 0, map_height_ - line_end_.Y));
                    long edTime = TimeUtility.Instance.GetElapsedTimeUs();

                    long stTime2 = TimeUtility.Instance.GetElapsedTimeUs();
                    int stRow, stCol, edRow, edCol;
                    spatial_system_.GetCell(new Vector3(line_start_.X, 0, map_height_ - line_start_.Y), out stRow, out stCol);
                    spatial_system_.GetCell(new Vector3(line_end_.X, 0, map_height_ - line_end_.Y), out edRow, out edCol);
                    //List<CellPos> path = new List<CellPos>();
                    //tree_cache_finder_.GetPath(new CellPos(stRow, stCol), new CellPos(edRow, edCol), path);
                    found_path_ = jump_point_finder_.FindPath(new CellPos(stRow, stCol), new CellPos(edRow, edCol));
                    long edTime2 = TimeUtility.Instance.GetElapsedTimeUs();

                    long stTime3 = TimeUtility.Instance.GetElapsedTimeUs();
                    found_path_ = spatial_system_.FindPath(new Vector3(line_start_.X, 0, map_height_ - line_start_.Y), new Vector3(line_end_.X, 0, map_height_ - line_end_.Y));
                    long edTime3 = TimeUtility.Instance.GetElapsedTimeUs();

                    /*//
                    found_path_.Clear();
                    if (path.Count > 0) {
                      foreach (CellPos p in path) {
                       found_path_.Add(spatial_system_.GetCellCenter(p.row, p.col));
                      }
                    }
                    ///*/
                    this.Text = "findpath:" + new Vector2(line_start_.X, map_height_ - line_start_.Y).ToString() + " to " + new Vector2(line_end_.X, map_height_ - line_end_.Y).ToString() + " consume " + (edTime - stTime) / 1000.0f + "ms no preprocess consume " + (edTime2 - stTime2) / 1000.0f + "ms triangulation network consume " + (edTime3 - stTime3) / 1000.0f + "ms";
                }
            }
            is_mouse_down_ = false;
            key_hit_ = 0;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!IsValid())
                return;
            line_start_ = new Point(e.X, e.Y);
            is_mouse_down_ = true;
            key_hit_ = 0;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsValid())
                return;
            if (is_mouse_down_)
            {
                switch (left_button_function_)
                {
                    case FunctionType.kMoveObj:
                        Vector3 new_pos = new Vector3(e.X, 0, map_height_ - e.Y);
                        control_obj_.SetPosition(new_pos);
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = @"D:/webjet/Public/Resource/Public/Scenes";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                imageFile.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            map_image_ = imageFile.Text;
            string path = Path.GetDirectoryName(map_image_);
            obstacle_file_ = Path.ChangeExtension(map_image_, "tmx");
            obs_file_ = Path.ChangeExtension(map_image_, "obs");
            nav_file_ = Path.ChangeExtension(map_image_, "navmesh");
            map_file_ = Path.ChangeExtension(map_image_, "map");
            path_file_ = "test.path"; //Path.ChangeExtension(map_image_, "path");
            map_patch_file_ = map_file_ + ".patch";
            map_actual_width_ = float.Parse(mapWidth.Text);

            Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            map_patch_parser_.Save(map_patch_file_);
        }

        private void operation_SelectedIndexChanged(object sender, EventArgs e)
        {
            hit_points_.Clear();
            left_button_function_ = (FunctionType)operation.SelectedIndex;
        }

        private void Tick(long deltaTime)
        {
            bool isMoving = true;
            double dir = 0;
            switch (key_hit_)
            {
                case KeyHit.W:
                    dir = Math.PI;
                    break;
                case KeyHit.A:
                    dir = Math.PI * 3 / 2;
                    break;
                case KeyHit.S:
                    dir = 0;
                    break;
                case KeyHit.D:
                    dir = Math.PI / 2;
                    break;
                case KeyHit.W | KeyHit.A:
                    dir = Math.PI * 5 / 4;
                    break;
                case KeyHit.W | KeyHit.D:
                    dir = Math.PI * 3 / 4;
                    break;
                case KeyHit.S | KeyHit.A:
                    dir = Math.PI * 7 / 4;
                    break;
                case KeyHit.S | KeyHit.D:
                    dir = Math.PI / 4;
                    break;
                default:
                    isMoving = false;
                    break;
            }
            if (isMoving)
            {
                if (!Geometry.IsSameFloat((float)dir, last_movedir_))
                {
                    last_adjust_ = 0;
                }
                DoMove(dir, deltaTime);
                last_movedir_ = (float)dir;
            }
            else
            {
                last_adjust_ = 0;
            }
        }

        private void DoMove(double dir, long deltaTime)
        {
            double PI_1 = Math.PI;
            double PI_2 = 2 * Math.PI;
            double forecastDistance = 0.75 * cell_width_;
            double speed = 5.0 * cell_width_ / 0.5;
            Vector3 old_pos = control_obj_.GetPosition();
            Vector3 new_pos = new Vector3();
            double distance = (speed * deltaTime) / 1000;

            for (int ct = 0; ct < 8; ++ct)
            {
                double cosV = Math.Cos(dir);
                double sinV = Math.Sin(dir);
                double y = old_pos.z + distance * cosV;
                double x = old_pos.x + distance * sinV;
                double checkY, checkX;
                if (distance < forecastDistance)
                {
                    checkY = old_pos.z + forecastDistance * cosV;
                    checkX = old_pos.x + forecastDistance * sinV;
                }
                else
                {
                    checkY = y;
                    checkX = x;
                }
                    checkY = y;
                    checkX = x;

                new_pos = new Vector3((float)x, 0, (float)y);
                Vector3 check_pos = new Vector3((float)checkX, 0, (float)checkY);
                if (spatial_system_.CanPass(control_obj_, check_pos))
                {
                    //正常移动
                    control_obj_.SetPosition(new_pos);
                    if(cell_manager_.GetCellStatus(new_pos) == BlockType.STATIC_BLOCK)
                    {
                        LogUtil.Info("ERROR !");
                    }
                    break;
                }
                else
                {
                    //自动调节方向
                    double newDir = (dir + PI_1 / 4) % PI_2;
                    if (last_adjust_ >= 0 && CanGo(old_pos.x, old_pos.z, newDir, forecastDistance))
                    {
                        LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", dir, newDir, last_adjust_);
                        dir = newDir;
                        last_adjust_ = 1;
                    }
                    else
                    {
                        LogUtil.Info("adjust dir:{0}->{1} failed, last adjust:{2}", dir, newDir, last_adjust_);
                        newDir = (dir + PI_2 - PI_1 / 4) % PI_2;
                        if (last_adjust_ <= 0 && CanGo(old_pos.x, old_pos.z, newDir, forecastDistance))
                        {
                            LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", dir, newDir, last_adjust_);
                            dir = newDir;
                            last_adjust_ = -1;
                        }
                        else
                        {
                            LogUtil.Info("adjust dir:{0}->{1} failed, last adjust:{2}", dir, newDir, last_adjust_);
                            newDir = (dir + PI_1 / 2) % PI_2;
                            if (last_adjust_ >= 0 && CanGo(old_pos.x, old_pos.z, newDir, forecastDistance))
                            {
                                LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", dir, newDir, last_adjust_);
                                dir = newDir;
                                last_adjust_ = 1;
                            }
                            else
                            {
                                LogUtil.Info("adjust dir:{0}->{1} failed, last adjust:{2}", dir, newDir, last_adjust_);
                                newDir = (dir + PI_2 - PI_1 / 2) % PI_2;
                                if (last_adjust_ <= 0 && CanGo(old_pos.x, old_pos.z, newDir, forecastDistance))
                                {
                                    LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", dir, newDir, last_adjust_);
                                    dir = newDir;
                                    last_adjust_ = -1;
                                }
                                else
                                {
                                    LogUtil.Info("adjust dir:{0}->{1} failed, last adjust:{2}", dir, newDir, last_adjust_);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool CanGo(float x, float y, double dir, double forecastDistance)
        {
            double cosV = Math.Cos(dir);
            double sinV = Math.Sin(dir);
            float tryY = (float)(y + forecastDistance * cosV);
            float tryX = (float)(x + forecastDistance * sinV);
            Vector3 spos = new Vector3(x, 0, y);
            Vector3 dpos = new Vector3(tryX, 0, tryY);
            bool ret = spatial_system_.CanPass(spos, dpos);
            int srow, scol, drow, dcol;
            cell_manager_.GetCell(spos, out srow, out scol);
            cell_manager_.GetCell(dpos, out drow, out dcol);
            if (ret)
            {
                LogUtil.Info("CanGo return true, ({0},{1})->({2},{3})", srow, scol, drow, dcol);
            }
            else
            {
                LogUtil.Info("CanGo return false, ({0},{1})->({2},{3})", srow, scol, drow, dcol);
            }
            return ret;
        }

        private void Reset()
        {
            tiled_map_parser_ = new TiledMapParser();
            nav_map_parser_ = new NavmeshMapParser();
            map_patch_parser_ = new MapPatchParser();
            spatial_system_ = new Spatial.SpatialSystem();
            cell_manager_ = new CellManager();

            back_image_ = Image.FromFile(map_image_);
            pictureBox1.Image = back_image_;
            pictureBox1.Size = back_image_.Size;
            map_width_ = back_image_.Width;
            map_height_ = back_image_.Height;

            context_.MaximumBuffer = new Size((int)map_width_, (int)map_height_);
            Graphics targetDC = this.pictureBox1.CreateGraphics();
            graphics_ = context_.Allocate(targetDC, new Rectangle(0, 0, (int)map_width_, (int)map_height_));
            obstacle_graphics_ = context_.Allocate(targetDC, new Rectangle(0, 0, (int)map_width_, (int)map_height_));

            scale = map_width_ / map_actual_width_;
            cell_width_ = cell_actual_width_ * scale;
            if (cell_width_ < 1)
                cell_width_ = 1;
            map_actual_height_ = map_height_ * map_actual_width_ / map_width_;

            ///*
            cell_manager_.Init(map_actual_width_, map_actual_height_, cell_actual_width_);
            cell_manager_.Scale(map_width_ / map_actual_width_);
            nav_map_parser_.ParseTileDataWithNavmeshUseFileScale(nav_file_, map_actual_width_, map_actual_height_);
            nav_map_parser_.GenerateObstacleInfoWithNavmesh(cell_manager_);

            //map_parser_.ParseTiledData(obstacle_file_, map_width_, map_height_);
            //map_parser_.GenerateObstacleInfo(cell_manager_);
            //map_parser_.SaveObstacleInfo(obs_file_);
            cell_manager_.Scale(map_actual_width_ / map_width_);
            cell_manager_.Save(map_file_);
            //tree_cache_finder_.PreprocessMap(cell_manager_, path_file_);
            //tree_cache_finder_.PrepareForSearch(path_file_);
            jump_point_finder_ = new JumpPointFinder();
            jump_point_finder_.Init(cell_manager_);
            //jump_point_finder_.PreprocessMap(path_file_);
            //*/

            spatial_system_.Init(map_file_, path_file_);
            long stTime = TimeUtility.Instance.GetElapsedTimeUs();
            spatial_system_.LoadObstacle(obs_file_, true);
            spatial_system_.TriangulationNetworkFinder.SparseDistance = 16.0f;
            triangles = spatial_system_.TriangulationNetworkFinder.TriangulationNetwork;
            long edTime = TimeUtility.Instance.GetElapsedTimeUs();
            LogUtil.Info("triangulation consume " + (edTime - stTime) + "us");

            spatial_system_.JumpPointFinder.RecordVisitedCells = false;
            cell_manager_ = spatial_system_.GetCellManager();
            cell_manager_.Scale(scale);

            jump_point_finder_.Init(cell_manager_);

            map_patch_parser_.Load(map_patch_file_);
            map_patch_parser_.VisitPatches((int row, int col, byte obstacle) =>
            {
                cell_manager_.SetCellStatus(row, col, obstacle);
                LogUtil.Info("map patch:{0},{1}->{2}", row, col, obstacle);
            });

            control_obj_.SetPosition(new Vector3(0, 0, 0));
            spatial_system_.AddObj(control_obj_);

            prkdtree_ = new PrKdTree(map_width_);
            kdtree_.Clear();
            space_objs_.Clear();
            selected_objs_.Clear();
            selected_objs2_.Clear();
            hit_points_.Clear();
            found_path_ = null;

            UpdateObstacleGraph();
            hit_points_.Clear();
        }

        private void UpdateObstacleGraph()
        {
            //绘制静态阻挡
            obstacle_graphics_.Graphics.Clear(Color.Black);
            obstacle_graphics_.Graphics.DrawImage(back_image_, 0, 0, map_width_, map_height_);
            DrawBlockSquare();
        }

        private void DrawBlockSquare()
        {
            for (int row = 0; row < cell_manager_.GetMaxRow(); ++row)
            {
                for (int col = 0; col < cell_manager_.GetMaxCol(); ++col)
                {
                    Vector3 pos = cell_manager_.GetCellCenter(row, col);
                    byte status = cell_manager_.GetCellStatus(row, col);
                    byte block = BlockType.GetBlockType(status);
                    DrawSquare((int)pos.x, (int)(map_height_ - pos.z), (int)cell_width_, block);
                }
            }
        }

        private void DrawSquare(int x, int y, int width, byte block)
        {
            if (block != BlockType.NOT_BLOCK)
            {
                obstacle_graphics_.Graphics.FillRectangle(brush, x - width / 2, y - width / 2, width, width);
            }
            if (block != BlockType.NOT_BLOCK)
            {
                obstacle_graphics_.Graphics.DrawRectangle(pen, x - width / 2, y - width / 2, width, width);
            }
        }

        private void DrawLine()
        {
            if (!is_mouse_down_)
            {
                graphics_.Graphics.DrawLine(line_pen, line_start_, line_end_);
            }
        }

        private void DrawPath()
        {
            if (null != found_path_ && found_path_.Count >= 2)
            {
                bool isFirst = true;
                Point one = new Point(), two = new Point();
                foreach (Vector3 pos in found_path_)
                {
                    Point pt = new Point((int)pos.x, (int)(map_height_ - pos.z));
                    if (isFirst)
                    {
                        one = pt;
                        isFirst = false;
                    }
                    else
                    {
                        two = pt;
                        graphics_.Graphics.DrawLine(path_pen, one, two);
                        graphics_.Graphics.FillEllipse(path_brush, two.X - cell_width_, two.Y - cell_width_, cell_width_ * 2, cell_width_ * 2);
                        one = two;
                    }
                }
            }
        }

        private void DrawObjs()
        {
            int ct = space_objs_.Count;
            for (int i = 0; i < ct; ++i)
            {
                TestSpaceObject obj = space_objs_[i];
                Vector3 pos = obj.Position;
                graphics_.Graphics.FillEllipse(space_obj_brush, pos.x - cell_width_, pos.z - cell_width_, cell_width_ * 2, cell_width_ * 2);
                graphics_.Graphics.DrawEllipse(space_obj_pen, pos.x - cell_width_, pos.z - cell_width_, cell_width_ * 2, cell_width_ * 2);
            }

            prkdtree_.VisitTree((float x0, float z0, float x1, float z1, int begin, int end, PrKdTreeObject[] objs) =>
            {
                graphics_.Graphics.DrawLine(prkdtree_pen, x0, z0, x1, z1);
            });

            kdtree_.VisitTree((float x0, float z0, float x1, float z1, int begin, int end, KdTreeObject[] objs) =>
            {
                //graphics_.Graphics.DrawLine(kdtree_pen, x0, z0, x1, z1);
                graphics_.Graphics.DrawLine(kdtree_pen, x0, z0, x0, z1);
                graphics_.Graphics.DrawLine(kdtree_pen, x0, z1, x1, z1);
                graphics_.Graphics.DrawLine(kdtree_pen, x1, z1, x1, z0);
                graphics_.Graphics.DrawLine(kdtree_pen, x1, z0, x0, z0);
            });

            /*
            spatial_system_.TriangulationNetworkFinder.KdTree.VisitTree((float x0, float z0, float x1, float z1, int begin, int end, TriangleNode[] objs) =>
            {
                graphics_.Graphics.DrawLine(kdgeotree_pen, x0, z0, x1, z1);
            });
            */

            ct = selected_objs_.Count;
            for (int i = 0; i < ct; ++i)
            {
                TestSpaceObject obj = selected_objs_[i];
                Vector3 pos = obj.Position;
                graphics_.Graphics.FillEllipse(prkdtree_select_brush, pos.x - cell_width_, pos.z - cell_width_ / 2, cell_width_, cell_width_);
            }

            ct = selected_objs2_.Count;
            for (int i = 0; i < ct; ++i)
            {
                TestSpaceObject obj = selected_objs2_[i];
                Vector3 pos = obj.Position;
                graphics_.Graphics.FillEllipse(kdtree_select_brush, pos.x, pos.z - cell_width_ / 2, cell_width_, cell_width_);
            }
        }

        private void DrawHitPoints()
        {
            foreach (CellPos p in hit_points_)
            {
                Vector3 pos = Transform(cell_manager_.GetCellCenter(p.row, p.col));
                graphics_.Graphics.FillRectangle(point_brush, pos.x - cell_width_ / 2, pos.z - cell_width_ / 2, cell_width_, cell_width_);
                graphics_.Graphics.DrawRectangle(point_pen, pos.x - cell_width_ / 2, pos.z - cell_width_ / 2, cell_width_, cell_width_);
            }
        }

        private void DrawMarkerSquare()
        {
            if (null != spatial_system_.JumpPointFinder)
            {
                int ct = spatial_system_.JumpPointFinder.VisitedCells.Count;
                for (int i = 0; i < ct; ++i)
                {
                    CellPos cell = spatial_system_.JumpPointFinder.VisitedCells[i];
                    Vector3 pos = cell_manager_.GetCellCenter(cell.row, cell.col);
                    DrawMarker((int)pos.x, (int)(map_height_ - pos.z), (int)cell_width_);
                }
            }
            if (null != triangles)
            {
                for (int i = 0; i < triangles.Count; ++i)
                {
                    TriangleNode tri = triangles[i];
                    Vector3 pt1 = tri.Points[0];
                    Vector3 pt2 = tri.Points[1];
                    Vector3 pt3 = tri.Points[2];
                    Point p1 = new Point((int)pt1.x, (int)(map_height_ - pt1.z));
                    Point p2 = new Point((int)pt2.x, (int)(map_height_ - pt2.z));
                    Point p3 = new Point((int)pt3.x, (int)(map_height_ - pt3.z));
                    graphics_.Graphics.DrawLine(triangle_pen, p1, p2);
                    graphics_.Graphics.DrawLine(triangle_pen, p2, p3);
                    graphics_.Graphics.DrawLine(triangle_pen, p3, p1);
                    graphics_.Graphics.DrawString((i + 1).ToString(), SystemFonts.DefaultFont, triangle_brush, (int)tri.Position.x, (int)(map_height_ - tri.Position.z));
                    //graphics_.Graphics.FillPolygon(triangle_brush, new Point[] { p1, p2, p3, p1 });
                }
            }
        }

        private void DrawMarker(int x, int y, int width)
        {
            graphics_.Graphics.DrawEllipse(marker_pen, x - width / 2, y - width / 2, width, width);
        }

        private bool IsValid()
        {
            bool ret = true;
            if (null == graphics_ ||
              null == back_image_ ||
              null == tiled_map_parser_ ||
              null == nav_map_parser_ ||
              null == cell_manager_ ||
              null == spatial_system_)
                ret = false;
            return ret;
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            long curTime = TimeUtility.Instance.GetElapsedTimeUs() / 1000;
            long deltaTime = curTime - last_tick_time_;
            last_tick_time_ = curTime;
            if (null != graphics_ && null != back_image_)
            {
                Tick(deltaTime);

                graphics_.Graphics.Clear(Color.White);
                obstacle_graphics_.Render(graphics_.Graphics);
                DrawHitPoints();
                DrawLine();
                DrawPath();
                DrawObjs();
                DrawMarkerSquare();
                UpdateObstacleGraph();
                Vector3 pos = Transform(control_obj_.GetPosition());
                graphics_.Graphics.FillEllipse(obj_brush, pos.x - cell_width_, pos.z - cell_width_, cell_width_ * 2, cell_width_ * 2);
                graphics_.Graphics.DrawEllipse(obj_pen, pos.x - cell_width_, pos.z - cell_width_, cell_width_ * 2, cell_width_ * 2);
                graphics_.Render();
            }
        }

        private Vector3 Transform(Vector3 pos)
        {
            return new Vector3(pos.x, pos.y, map_height_ - pos.z);
        }
    }
}
