using MudBlazor;
using System;
using System.Collections.Generic;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentInputForm.DataObject
{
    public class InputFormDataObject
    {
        public List<InputData> InputDatas;
        public List<ButtonData> ButtonDatas;

        public InputFormDataObject()
        {
            InputDatas = new List<InputData>();
            ButtonDatas = new List<ButtonData>();
        }

        public void AddInputData(params InputData[] datas)
        {
            foreach (var data in datas)
            {
                InputDatas.Add(data);
            }
        }
        public void AddButtonData(params ButtonData[] datas)
        {
            foreach (var data in datas)
            {
                ButtonDatas.Add(data);
            }
        }
    }

    public class ButtonData
    {
        public ButtonData()
        {
        }

        public ButtonData(string buttonText)
        {
            ButtonText = buttonText;
        }

        public string ButtonText { get; set; }
        public Action<ButtonData> EventButtonClick { get; set; }

        public void OnEventClick(ButtonData buttonData)
        {
            EventButtonClick?.Invoke(buttonData);
        }
    }

    public class InputData
    {
        public InputData()
        {
        }

        public InputData(string inputDescText, string inputDescIcon, InputType inputType)
        {
            InputDescText = inputDescText;
            InputDescIcon = inputDescIcon;
            InputType = inputType;
        }

        public string InputDescText { get; set; } = "";
        public string InputDescIcon { get; set; } = "";
        public InputType InputType { get; set; } = InputType.Text;
        public string InputText { get; set; } = "";


        public string GetInputIcon()
        {
            if (string.IsNullOrEmpty(InputDescIcon))
            {
                return "";
            }
            return InputDescIcon;
        }
    }
}
