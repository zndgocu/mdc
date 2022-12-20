using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.doosan.fms.webapp.Client.ComponentLibrary.ComponentCardGrid.DataObject
{
    public class CardGridDataObjectProperties
    {
        public int Xs { get; set; }
        public Justify Justify { get; set; }
        public CardGridDataObjectProperties()
        {
            Xs = 12;
            Justify = Justify.FlexStart;
        }

        public CardGridDataObjectProperties(int xs)
        {
            Xs = xs;
            Justify = Justify.FlexStart;
        }

        public CardGridDataObjectProperties(int xs, Justify justify) : this(xs)
        {
            Justify = justify;
        }
    }

    public class CardData
    {
        public CardData()
        {
            Xs = 2;
            Text = "";
            Icon = "";
        }

        public CardData(int xs, string text, string icon)
        {
            Xs = xs;
            Text = text;
            Icon = icon;
        }

        public int Xs { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }


    public class CardGridDataObject
    {
        public event Func<CardData, Task<bool>> EventCardClick;
        public async Task<bool> OnEventCardClick(CardData arg)
        {
            if (EventCardClick == null) return false;
            return await EventCardClick.Invoke(arg);
        }

        public CardGridDataObjectProperties CardGridProperties;
        public List<CardData> CardData;

        public CardGridDataObject(CardGridDataObjectProperties cardGridProperties, List<CardData> cardData)
        {
            CardGridProperties = cardGridProperties;
            CardData = cardData;
        }
    }
}
