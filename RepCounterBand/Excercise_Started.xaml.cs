using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using System.Threading.Tasks;
using Windows.UI.Core;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace RepCounterBand
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Excercise_Started : Page
    {
        // Passed Navigation Querystring for Exercise Name
        public static string ExerciseName { get; set; }

        // Create X Acceleration Axis
        AccelerationAxis oXAccelerationAxis = new AccelerationAxis(AxisType.X);
        // Create Y Acceleration Axis
        AccelerationAxis oYAccelerationAxis = new AccelerationAxis(AxisType.Y);
        // Create Z Acceleration Axis
        AccelerationAxis oZAccelerationAxis = new AccelerationAxis(AxisType.Z);

        public double XVectorThreshold1_1 { get; set; }
        public double XVectorThreshold1_2 { get; set; }

        public double XVectorThreshold2_1 { get; set; }
        public double XVectorThreshold2_2 { get; set; }

        public double YVectorThreshold1_1 { get; set; }
        public double YVectorThreshold1_2 { get; set; }

        public double YVectorThreshold2_1 { get; set; }
        public double YVectorThreshold2_2 { get; set; }

        public double ZVectorThreshold1_1 { get; set; }
        public double ZVectorThreshold1_2 { get; set; }

        public double ZVectorThreshold2_1 { get; set; }
        public double ZVectorThreshold2_2 { get; set; }


        public ThresholdDetectEnum ThresholdTarget { get; set; }
        //llamba function
        public delegate bool CompareDelegate(double x, double y);
        public CompareDelegate CompareLambdaExpx1_1 { get; set; }
        public CompareDelegate CompareLambdaExpx1_2 { get; set; }
        public CompareDelegate CompareLambdaExpx2_1 { get; set; }
        public CompareDelegate CompareLambdaExpx2_2 { get; set; }
        public CompareDelegate CompareLambdaExpy1_1 { get; set; }
        public CompareDelegate CompareLambdaExpy1_2 { get; set; }
        public CompareDelegate CompareLambdaExpy2_1 { get; set; }
        public CompareDelegate CompareLambdaExpy2_2 { get; set; }
        public CompareDelegate CompareLambdaExpz1_1 { get; set; }
        public CompareDelegate CompareLambdaExpz1_2 { get; set; }
        public CompareDelegate CompareLambdaExpz2_1 { get; set; }
        public CompareDelegate CompareLambdaExpz2_2 { get; set; }

        // Variables used in Calorie calcuation
        public static int Hours { get; set; }
        public static int Mins { get; set; }
        public static int Secs { get; set; }
        public static int MSecs { get; set; }
        public static double Weight_lbs { get; set; }
        public static double Weight_kgs { get; set; }
        public static double Product_MET_kilo { get; set; }
        public static double MET_Value { get; set; }
        public static double ProductofAll { get; set; }
        public static double CaloriesPerMinute { get; set; }
        public static double PartOfMinute { get; set; }
        public static double TotalMins { get; set; }
        public static double Magnitude { get; set; }
        public static double MagnitudeMultiplied { get; set; }
        public double XmlMetValue { get; set; }

        // Other variables
        public static int trigsecs { get; set; }
        public static int timeout_setting { get; set; }
        public static bool stoptrig { get; set; }
        public static string placeholderzero { get; set; }
        public string x1_1 { get; set; }
        public string x1_2 { get; set; }
        public string x2_1 { get; set; }
        public string x2_2 { get; set; }
        public string y1_1 { get; set; }
        public string y1_2 { get; set; }
        public string y2_1 { get; set; }
        public string y2_2 { get; set; }
        public string z1_1 { get; set; }
        public string z1_2 { get; set; }
        public string z2_1 { get; set; }
        public string z2_2 { get; set; }
        public string symx1_1 { get; set; }
        public string symx1_2 { get; set; }
        public string symx2_1 { get; set; }
        public string symx2_2 { get; set; }
        public string symy1_1 { get; set; }
        public string symy1_2 { get; set; }
        public string symy2_1 { get; set; }
        public string symy2_2 { get; set; }
        public string symz1_1 { get; set; }
        public string symz1_2 { get; set; }
        public string symz2_1 { get; set; }
        public string symz2_2 { get; set; }
        public string w_type { get; set; }
        public string timeout_enabled { get; set; }
        public string helpid { get; set; }
        public bool question_page_back { get; set; }

        public int alreadystartedflag { get; set; }
        public int poststart { get; set; }
        public static bool lockedflag { get; set; }
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer dispatcherTimer_trig;
        public Excercise_Started()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //show the intro layer can be disabled for debug
            Intro_layer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //start timer and set-up varibles
            Hours = 0; Mins = 0; Secs = 0; MSecs = 0; trigsecs = 0;
            //sets timeout value in future load from xml
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //read secs value from settings storage
            Object secs = settings.Values["secs"];
            if (secs == null)
            {
                timeout_setting = 5;
            }
            else
            {
                string time_secs = secs.ToString();
                timeout_setting = Convert.ToInt32(time_secs);
            }
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer_trig = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            dispatcherTimer_trig.Tick += new EventHandler<object>(dispatcherTimer_trig_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer_trig.Interval = new TimeSpan(0, 0, timeout_setting);
            //set post start status
            poststart = 0;
            //tell the app that we are not on the help virtual page
            question_page_back = false;

            //calculate kgs weight from lbs
            //read weight value and typefrom settings storage create failsafe if no value
            Object weight = settings.Values["Weight"];
            Object weight_type = settings.Values["weight-type"];
            if (weight == null)
            {
                settings.Values["Weight"] = "160";
            }
            else
            {
              //leave it alone if value present
            }
            if (weight_type == null)
            {
                settings.Values["weight-type"] = "lbs";
            }
            else
            {
                //leave it alone if value present
            }

            // set value based on value type
            w_type = settings.Values["weight-type"].ToString();
                if (w_type == "lbs")
                {
                    string wlbs = settings.Values["Weight"].ToString();
                    Weight_kgs = Convert.ToInt32(wlbs) / 2.2;
                }
                else
                {
                    string wt = settings.Values["Weight"].ToString();
                    Weight_kgs = Convert.ToDouble(wt);
                }


            //load the goal previous numbers for easy selection
            //get time goal
            Object time_goal = settings.Values["time-goal"];
            if (time_goal == null)
            {
                settings.Values["time-goal"] = "30:00";
            }
            else
            {
                txtbox_duration.Text = settings.Values["time-goal"].ToString();
            }
            //get count goal
            Object count_goal = settings.Values["count-goal"];
            if (count_goal == null)
            {
                txtbox_reps.Text = "200";
            }
            else
            {
                txtbox_reps.Text = settings.Values["count-goal"].ToString();
            }
            //get calorie goal
            Object calorie_goal = settings.Values["calorie-goal"];
            if (count_goal == null)
            {
                txtbox_cals.Text = "100";
            }
            else
            {
                txtbox_cals.Text = settings.Values["calorie-goal"].ToString();
            }
        }
        //on navigated too start trying to find band and get data
        private async void OnNavigatedTo(object sender, NavigationEventArgs e)
        {
            try
            {
                // Get the list of Microsoft Bands paired to the phone.
                IBandInfo[] pairedBands = await BandClientManager.Instance.GetBandsAsync();
                if (pairedBands.Length < 1)
                {
                  //message box here
                    return;
                    //add logic to open and pair on pair page here
                }

                // Connect to Microsoft Band.
                using (IBandClient bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    // Subscribe to Accelerometer data.
                    bandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                    await bandClient.SensorManager.Accelerometer.StartReadingsAsync();

                    // Receive Accelerometer data for a while.
                    //await Task.Delay(TimeSpan.FromMinutes(1));
                    //await bandClient.SensorManager.Accelerometer.StopReadingsAsync(); use to stop getting data
                }
            }
            catch (Exception ex)
            {
                //this.textBlock.Text = ex.ToString();
            }
        }
        private async void Accelerometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            IBandAccelerometerReading accel = e.SensorReading;
            string text = string.Format("X = {0}\nY = {1}\nZ = {2}", accel.AccelerationX, accel.AccelerationY, accel.AccelerationZ);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { this.textBlock.Text = text; }).AsTask();

            //multiply MET value by weight in kilograms and by 3.5
            Product_MET_kilo = MET_Value * 3.5 * Weight_kgs;

            // Get calories per minute
            CaloriesPerMinute = Product_MET_kilo / 200;

            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            //read secs value from settings storage
            Object secs = settings.Values["secs"];

            if (stoptrig == true)
            {
                string timeout_enabled = settings.Values["timeout-checked"].ToString();
                if (timeout_enabled == "true")
                {
                    dispatcherTimer_trig.Start();
                }
            }
            if (Secs <= 58)
            {
                PartOfMinute = Secs * 0.01666667;
            }
            else
                PartOfMinute = 0;

            if (ThresholdTarget == ThresholdDetectEnum.THRESHOLD_1)
            {
                /******************************************************************
                 * Determine if we hit Threshold 1 based off of vector acceleration
                 * ****************************************************************/

                if (
                    // X range set
                    EvalLambda(accel.AccelerationX, XVectorThreshold1_1, CompareLambdaExpx1_1)
                    && EvalLambda(accel.AccelerationX, XVectorThreshold1_2, CompareLambdaExpx1_2)
                    // Y Range set
                    && EvalLambda(accel.AccelerationY, YVectorThreshold1_1, CompareLambdaExpy1_1)
                    && EvalLambda(accel.AccelerationY, YVectorThreshold1_2, CompareLambdaExpy1_2)
                    // Z Range set
                    && EvalLambda(accel.AccelerationZ, ZVectorThreshold1_1, CompareLambdaExpz1_1)
                    && EvalLambda(accel.AccelerationZ, ZVectorThreshold1_2, CompareLambdaExpz1_2)
                    )
                {

                    // Set new threshold target to THRESHOLD_2
                    ThresholdTarget = ThresholdDetectEnum.THRESHOLD_2;
                    //stoptrig = true;
                    //reset pause trigger counter and hide paused screen
                    Object time_checked = settings.Values["timeout-checked"];
                    string timeout_checked = time_checked.ToString();
                  
                    if (timeout_checked != null)
                    {
                        timeout_enabled = settings.Values["timeout-checked"].ToString();

                        if (timeout_enabled == "true")
                        {
                            dispatcherTimer_trig.Stop();
                            dispatcherTimer_trig.Start();
                        }
                    }
                    else
                    {
                        if (timeout_enabled == "false")
                        {

                        }
                        else
                        {
                            dispatcherTimer_trig.Stop();
                            dispatcherTimer_trig.Start();
                        }
                    }
                    btn_pause.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    pause_layer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    Status_Text.Text = "Active";
                    if (lockedflag == true)
                    {
                        btn_reset.IsEnabled = false;
                        btn_stop.IsEnabled = false;
                    }


                }
                else
                {
                    //do nothing or pause logic here
                }
        }
        
            else
            {
                if (ThresholdTarget == ThresholdDetectEnum.THRESHOLD_2)
                {
                    /******************************************************************
                     * Determine if we hit threshold 2 based off of vector acceleration
                     * ****************************************************************/
                    if (
                        // X range set
                        EvalLambda(accel.AccelerationX, XVectorThreshold2_1, CompareLambdaExpx2_1)
                        && EvalLambda(accel.AccelerationX, XVectorThreshold2_2, CompareLambdaExpx2_2)
                        // Y range set
                        && EvalLambda(accel.AccelerationY, YVectorThreshold2_1, CompareLambdaExpy2_1)
                        && EvalLambda(accel.AccelerationY, YVectorThreshold2_2, CompareLambdaExpy2_2)
                        //z range set
                        && EvalLambda(accel.AccelerationZ, ZVectorThreshold2_1, CompareLambdaExpz2_1)
                        && EvalLambda(accel.AccelerationZ, ZVectorThreshold2_2, CompareLambdaExpz2_2)
                        )
                    {
                        // Increment Rep count only on second threshold!!!
                        RepCounterHelper.GlobalCount++;

                        // Update Rep count display
                        Count_Number.Text = RepCounterHelper.GlobalCount.ToString("");
                        //stop if no movement
                        if (!dispatcherTimer.IsEnabled)
                        {
                            dispatcherTimer.Start();
                        }
                        // Set new threshold target to THRESHOLD_1
                        ThresholdTarget = ThresholdDetectEnum.THRESHOLD_1;
                        //reset pause trigger counter
                        dispatcherTimer_trig.Stop();
                        dispatcherTimer_trig.Start();
                        btn_pause.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        pause_layer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        Status_Text.Text = "Active";
                        if (lockedflag == true)
                        {
                            btn_reset.IsEnabled = false;
                            btn_stop.IsEnabled = false;
                        }


                    }
                    else
                    {
                        //do nothing or pause logic here
                    }
                    Magnitude = Math.Sqrt(Math.Pow(accel.AccelerationX, 2) + Math.Pow(accel.AccelerationY, 2) + Math.Pow(accel.AccelerationZ, 2));
                    // Show the values graphically.
                    //this.chart.Series[0].ItemsSource = Magnitude;
                }

            }
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // Get Passed Parameters
                base.OnNavigatedTo(e);
                //ExerciseName = NavigationContext.QueryString["exerciseName"]; replace with new passing paremter from page logic
                ExerciseName = "Abdominal Crunch";
                Excercise_Title.Text = ExerciseName;

                // To Set the exercise from xml string
                XDocument oXDocument = RepCounterHelper.ExerciseXDocument;

                // Go to Excercise Level
                foreach (XElement oExcerciseXElement in oXDocument.Root
                                .Elements("Excercise"))
                {
                    //go to Parent element met_value and get the met value so we can calculate calories
                    XmlMetValue = Convert.ToDouble(oExcerciseXElement.Attribute("MetValue").Value);
                    helpid = oExcerciseXElement.Attribute("HelpID").Value;
                    //Pick that excercise by finding matching the attribute name this is for after main menu gives us the excercise
                    if ((string)oExcerciseXElement.Attribute("Name").Value == ExerciseName)
                    {
                        //Get the axis values

                        // Go to Parent Element Axis_Values
                        XElement oAxisValueXElement = oExcerciseXElement.Element("Axis_Values");

                        //set strings equal to child element in this case the axis values
                        x1_1 = oAxisValueXElement.Element("X_FirstThreshold_FirstValue").Value;
                        x1_2 = oAxisValueXElement.Element("X_FirstThreshold_SecondValue").Value;
                        x2_1 = oAxisValueXElement.Element("X_SecondThreshold_FirstValue").Value;
                        x2_2 = oAxisValueXElement.Element("X_SecondThreshold_SecondValue").Value;
                        y1_1 = oAxisValueXElement.Element("Y_FirstThreshold_FirstValue").Value;
                        y1_2 = oAxisValueXElement.Element("Y_FirstThreshold_SecondValue").Value;
                        y2_1 = oAxisValueXElement.Element("Y_SecondThreshold_FirstValue").Value;
                        y2_2 = oAxisValueXElement.Element("Y_SecondThreshold_SecondValue").Value;
                        z1_1 = oAxisValueXElement.Element("Z_FirstThreshold_FirstValue").Value;
                        z1_2 = oAxisValueXElement.Element("Z_FirstThreshold_SecondValue").Value;
                        z2_1 = oAxisValueXElement.Element("Z_SecondThreshold_FirstValue").Value;
                        z2_2 = oAxisValueXElement.Element("Z_SecondThreshold_SecondValue").Value;

                        //get the lambda values

                        //go to Parent element lambda_symbol
                        XElement olambda_symbolXElement = oExcerciseXElement.Element("lambda_symbol");
                        //find the lamba symbols from the tree
                        symx1_1 = olambda_symbolXElement.Element("symbol_x1_1").Value;
                        symx1_2 = olambda_symbolXElement.Element("symbol_x1_2").Value;
                        symx2_1 = olambda_symbolXElement.Element("symbol_x2_1").Value;
                        symx2_2 = olambda_symbolXElement.Element("symbol_x2_2").Value;
                        symy1_1 = olambda_symbolXElement.Element("symbol_y1_1").Value;
                        symy1_2 = olambda_symbolXElement.Element("symbol_y1_2").Value;
                        symy2_1 = olambda_symbolXElement.Element("symbol_y2_1").Value;
                        symy2_2 = olambda_symbolXElement.Element("symbol_y2_2").Value;
                        symz1_1 = olambda_symbolXElement.Element("symbol_z1_1").Value;
                        symz1_2 = olambda_symbolXElement.Element("symbol_z1_2").Value;
                        symz2_1 = olambda_symbolXElement.Element("symbol_z2_1").Value;
                        symz2_2 = olambda_symbolXElement.Element("symbol_z2_2").Value;

                    }
                }
            }
            catch { }
        }

        
        //each time the timer ticks update the display
        private void dispatcherTimer_Tick(object sender, object e)
        {
            // Dispatch timer kicks off every second
            Secs++;
            if (Secs > 59)
            {
                Secs = 0;
                Mins = Mins + 1;
                if (Mins > 59)
                {
                    Mins = 0;
                    Hours = Hours + 1;
                }
            }

            //update calorie count on each tick
            TotalMins = Mins + PartOfMinute;

            ProductofAll = CaloriesPerMinute * TotalMins;
            lblCalories.Text = ProductofAll.ToString("0" + " Calories");
            if (Mins >= 1)
            {
                // Displays the calories per minute - not working
                lblCaloriespermin.Text = CaloriesPerMinute.ToString("0" + " Calories");
            }
            //add the extra zero if less than 10 seconds
            if (Secs < 10)
            {
                placeholderzero = "0";
            }
            else
            {
                placeholderzero = "";
            }

            lblTimerDisplay.Text = Hours + ":" + Mins + ":" + placeholderzero + Secs;
            //livetile_static = Hours + ":" + Mins + ":" + placeholderzero + Secs;

            //call live tile update
            //IconicTileData oIcontile = new IconicTileData();
            //oIcontile.Title = "My Rep";
            //oIcontile.Count = RepCounterHelper.GlobalCount;

            //oIcontile.IconImage = new Uri("Assets/Tiles/Iconic/202x202.png", UriKind.Relative);
            //oIcontile.SmallIconImage = new Uri("Assets/Tiles/Iconic/110x110.png", UriKind.Relative);

            //if (MainPage.ExerciseName == null)
            //{
            //    oIcontile.WideContent1 = "No Active Excercise";
            //}
            //else
            //{
            //    oIcontile.WideContent1 = MainPage.ExerciseName;
            //}
            //oIcontile.WideContent2 = MainPage.livetile_static + " - " + "Stopped";
            //oIcontile.WideContent3 = MainPage.ProductofAll.ToString("0.0" + " Calories");

        }

        //second event handler for stop trigger timer
        private void dispatcherTimer_trig_Tick(object sender, object e)
        {
            //stop timer if the reps are paused

            dispatcherTimer.Stop();
            btn_pause.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //this.PausedFadeout.Begin(); add new animation to xaml
            Status_Text.Text = "Auto Paused";
            dispatcherTimer_trig.Stop();
            if (lockedflag == true)
            {
                btn_reset.IsEnabled = true;
                btn_stop.IsEnabled = true;
            }

        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            //make sure old data is zeroed out
            lblTimerDisplay.Text = "0:00:00";
            lblStartTime.Text = "--:--:--";
            Count_Number.Text = "0";
            btn_question.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if (alreadystartedflag == 1)
            {
                //moAccelerometer = null;
            }
            //hide start and show pause, stop, reset buttons
            btn_start.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            btn_reset.Visibility = Windows.UI.Xaml.Visibility.Visible;
            btn_stop.Visibility = Windows.UI.Xaml.Visibility.Visible;
            btn_pause.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //make lock button visible
            btn_lock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            btn_reset.IsEnabled = true;
            btn_stop.IsEnabled = true;
            btn_pause.IsEnabled = true;
            lockedflag = false;
            //set status text
            Status_Text.Text = "Active";
            MagnitudeMultiplied = Magnitude / 3;
            //set MET Value based on excercise
            MET_Value = XmlMetValue - MagnitudeMultiplied;
            //reset global count and set x threshold values
            RepCounterHelper.GlobalCount = 0;
            //start define threshold values here

            // Set x Acceleration Axis values
            oXAccelerationAxis.AxisThresholds.FirstThreshold.FirstValue = Convert.ToDouble(x1_1);
            oXAccelerationAxis.AxisThresholds.FirstThreshold.SecondValue = Convert.ToDouble(x1_2);
            oXAccelerationAxis.AxisThresholds.SecondThreshold.FirstValue = Convert.ToDouble(x2_1);
            oXAccelerationAxis.AxisThresholds.SecondThreshold.SecondValue = Convert.ToDouble(x2_2);
            //Set Y Acceleration Axis values
            oYAccelerationAxis.AxisThresholds.FirstThreshold.FirstValue = Convert.ToDouble(y1_1);
            oYAccelerationAxis.AxisThresholds.FirstThreshold.SecondValue = Convert.ToDouble(y1_2);
            oYAccelerationAxis.AxisThresholds.SecondThreshold.FirstValue = Convert.ToDouble(y2_1);
            oYAccelerationAxis.AxisThresholds.SecondThreshold.SecondValue = Convert.ToDouble(y2_2);
            //Set Z Acceleration Axis values
            oZAccelerationAxis.AxisThresholds.FirstThreshold.FirstValue = Convert.ToDouble(z1_1);
            oZAccelerationAxis.AxisThresholds.FirstThreshold.SecondValue = Convert.ToDouble(z1_2);
            oZAccelerationAxis.AxisThresholds.SecondThreshold.FirstValue = Convert.ToDouble(z2_1);
            oZAccelerationAxis.AxisThresholds.SecondThreshold.SecondValue = Convert.ToDouble(z2_2);
            //end define threshold values

            // X 1-1
            if (symx1_1 == ">")
                CompareLambdaExpx1_1 = (x, y) => x > y;
            else
                CompareLambdaExpx1_1 = (x, y) => x < y;

            // X 1-2
            if (symx1_2 == ">")
                CompareLambdaExpx1_2 = (x, y) => x > y;
            else
                CompareLambdaExpx1_2 = (x, y) => x < y;

            // X 2-1
            if (symx2_1 == ">")
                CompareLambdaExpx2_1 = (x, y) => x > y;
            else
                CompareLambdaExpx2_1 = (x, y) => x < y;

            // X 2-2
            if (symx2_2 == ">")
                CompareLambdaExpx2_2 = (x, y) => x > y;
            else
                CompareLambdaExpx2_2 = (x, y) => x < y;

            // Y 1-1
            if (symy1_1 == ">")
                CompareLambdaExpy1_1 = (x, y) => x > y;
            else
                CompareLambdaExpy1_1 = (x, y) => x < y;

            // Y 1-2
            if (symy1_2 == ">")
                CompareLambdaExpy1_2 = (x, y) => x > y;
            else
                CompareLambdaExpy1_2 = (x, y) => x < y;

            // Y 2-1
            if (symy2_1 == ">")
                CompareLambdaExpy2_1 = (x, y) => x > y;
            else
                CompareLambdaExpy2_1 = (x, y) => x < y;

            // Y 2-2
            if (symy2_2 == ">")
                CompareLambdaExpy2_2 = (x, y) => x > y;
            else
                CompareLambdaExpy2_2 = (x, y) => x < y;

            // Z 1-1
            if (symz1_1 == ">")
                CompareLambdaExpz1_1 = (x, y) => x > y;
            else
                CompareLambdaExpz1_1 = (x, y) => x < y;

            // Z 1-2
            if (symz1_2 == ">")
                CompareLambdaExpz1_2 = (x, y) => x > y;
            else
                CompareLambdaExpz1_2 = (x, y) => x < y;

            // Z 2-1
            if (symz2_1 == ">")
                CompareLambdaExpz2_1 = (x, y) => x > y;
            else
                CompareLambdaExpz2_1 = (x, y) => x < y;

            // Z 2-2
            if (symz2_2 == ">")
                CompareLambdaExpz2_2 = (x, y) => x > y;
            else
                CompareLambdaExpz2_2 = (x, y) => x < y;

           
            //start the timer
            dispatcherTimer.Start();
            //set start time
            lblStartTime.Text = DateTime.Now.ToString("h:mm:ss tt");
            //it's now post start
            poststart = 1;
            //start timer that makes counter pause
            if (dispatcherTimer_trig.IsEnabled)
            {
                dispatcherTimer_trig.Stop();
            }
            dispatcherTimer_trig.Start();
        }
        public CompareDelegate LambdaExpBuilderLtGt(string in_sOperator)
        {
            if (in_sOperator == ">")
            {
                return (x, y) => x > y;
            }
            else
            {
                if (in_sOperator == "<")
                {
                    return (x, y) => x > y;
                }
            }
            return null;
        }



        public bool EvalLambdaOrig(double in_dbX
           , double in_dbY
           , Func<double, double, bool> op)
        {
            bool bResult = op(in_dbX, in_dbY);
            return bResult;
        }

        public bool EvalLambda(double in_dbX
            , double in_dbY
            , CompareDelegate in_CompareDelegate)
        {
            bool bResult = in_CompareDelegate(in_dbX, in_dbY);
            return bResult;
        }

    }
    // Non partial classes
    public enum ThresholdDetectEnum
    {
        THRESHOLD_1,
        THRESHOLD_2
    }

    public enum AxisType
    {
        X,
        Y,
        Z
    }

    public class ThresholdValues
    {
        public ThresholdValues()
        {
            FirstValue = 0;
            SecondValue = 0;
        }

        public double FirstValue { get; set; }
        public double SecondValue { get; set; }
    }


    public class Thresholds
    {
        // Constructor
        public Thresholds()
        {
            FirstThreshold = new ThresholdValues();
            SecondThreshold = new ThresholdValues();
        }

        public ThresholdValues FirstThreshold { get; set; }
        public ThresholdValues SecondThreshold { get; set; }
    }
    
    public class AccelerationAxis
    {
        // Constructor
        public AccelerationAxis(AxisType in_eAxisType)
        {
            AxisThresholds = new Thresholds();

            AxisType = in_eAxisType;
        }

        public Thresholds AxisThresholds { get; set; }
        public AxisType AxisType { get; set; }
    }
}