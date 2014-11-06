using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using AS.PDA.Util;
using AS.PDA.Model;
using AS.PDA.Service;
using Newtonsoft.Json;

using AS.PDA.Bracode;

namespace AS.PDA.View
{
    public partial class ApplyForm : Form
    {
        public ApplyForm()
        {
            InitializeComponent();
        }

        //System.EventHandler myActivateHandler = null;
        private barCodeAPI myScanBarCodeAPI = null;
        private EventHandler myReadNotifyHandler = null;
        private EventHandler myStatusNotifyHandler = null;
        private bool isReaderInitiated = false;


        HttpUtil util = new HttpUtil();
        HttpDataService httpDataService = new HttpDataService();

        private ConfigUtil configUtil = new ConfigUtil();


        private void ApplyForm_Load(object sender, EventArgs e)
        {
            this.myScanBarCodeAPI = new barCodeAPI();

            this.isReaderInitiated = this.myScanBarCodeAPI.InitReader();
            if (!(this.isReaderInitiated))// If the reader has not been initialized
            {
                // Display a message & exit the application.
                MessageBox.Show(Resources.GetString("AppExitMsg"));
                Application.Exit();
            }
            // If the reader has been initialized
            else
            {
                // Attach a status natification handler.
                this.myStatusNotifyHandler = new EventHandler(myReader_StatusNotify);
                myScanBarCodeAPI.AttachStatusNotify(myStatusNotifyHandler);
            }

            if (isReaderInitiated)
            {
                // Start a read operation & attach a handler.
                myScanBarCodeAPI.StartRead(false);
                this.myReadNotifyHandler = new EventHandler(myReader_ReadNotify);
                myScanBarCodeAPI.AttachReadNotify(myReadNotifyHandler);
            }

        }

        private void myReader_ReadNotify(object Sender, EventArgs e)
        {
            // Checks if the Invoke method is required because the ReadNotify delegate is called by a different thread
            if (this.InvokeRequired)
            {
                // Executes the ReadNotify delegate on the main thread
                this.Invoke(myReadNotifyHandler, new object[] { Sender, e });
            }
            else
            {
                // Get ReaderData
                Symbol.Barcode.ReaderData TheReaderData = this.myScanBarCodeAPI.Reader.GetNextReaderData();
                switch (TheReaderData.Result)
                {
                    case Symbol.Results.SUCCESS:
                        this.setHandleData(TheReaderData);
                        this.myScanBarCodeAPI.Reader.Actions.Read(TheReaderData);
                        break;
                    case Symbol.Results.CANCELED:
                        break;
                    default:
                        string sMsg = "Read Failed\n"
                        + "Result = " + ((int)TheReaderData.Result).ToString("X8");
                        break;
                }
            }
        }

        private void setHandleData(Symbol.Barcode.ReaderData TheReaderData)
        {
            txtBarcode.Text = TheReaderData.Text;
        }
        /// <summary>
        /// Status notification handler
        /// </summary>
        private void myReader_StatusNotify(object Sender, EventArgs e)
        {

            // Checks if the Invoke method is required because the StatusNotify delegate is called by a different thread
            if (this.InvokeRequired)
            {
                // Executes the StatusNotify delegate on the main thread
                this.Invoke(myStatusNotifyHandler, new object[] { Sender, e });
            }
            else
            {
                //// Get current status 
                Symbol.Barcode.BarcodeStatus TheEvent = this.myScanBarCodeAPI.Reader.GetNextStatus();

                //// Set event text in UI
                //this.txtBarcode.Text = TheEvent.Text;
            }
        }

        private void ApplyForm_Activated(object sender, EventArgs e)
        {
            this.myScanBarCodeAPI.StartRead(false);
            myScanBarCodeAPI.AttachStatusNotify(myStatusNotifyHandler);
        }

        private void ApplyForm_Deactivate(object sender, EventArgs e)
        {
            myScanBarCodeAPI.StopRead();
            myScanBarCodeAPI.DetachStatusNotify();
        }

          
        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {

            //检查网络是否正常
            try
            {
                if (SystemCache.ConnetionType == "NetWork")
                {
                    if (txtBarcode.Text.Trim().Length != 0)
                    {
                        string result = string.Empty;
                        string barcode = txtBarcode.Text.Trim().Substring(2, 6);
                        string positionName = configUtil.GetConfig("HttpConnectionStr")["positionName"];
                        if (httpDataService.FinishTask("/Transport/BarcodeArrive/?positionName=" + positionName + "&barcode=" + barcode, out result))
                        {
                           // MessageBox.Show(result);
                            txtBarcode.Text = "";
                        }
                        else
                        {
                            MessageBox.Show(result);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please Check the network");
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }      
    }
}