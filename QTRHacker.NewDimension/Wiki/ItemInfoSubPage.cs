﻿using Newtonsoft.Json.Linq;
using QTRHacker.NewDimension.Controls;
using QTRHacker.NewDimension.Res;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QTRHacker.NewDimension.Wiki
{
	public class ItemInfoSubPage : TabPage
	{
		private readonly Color ItemsColor = Color.FromArgb(160, 160, 200);
		public InfoView ItemIconInfoView, ItemNameInfoView, ItemTypeInfoView, ItemRareInfoView, ItemDescriptionInfoView, ItemRecipeFromInfoView, ItemRecipeToInfoView, ItemValueInfoView;
		public ListBox RecipeToItems;
		public TabControl RequireItems;

		public event Action<object, MouseEventArgs> OnRequireItemDoubleClick = (s, e) => { };
		public event Action<object, MouseEventArgs> OnRecipeToItemDoubleClick = (s, e) => { };

		public ItemInfoSubPage() : base(MainForm.CurrentLanguage["ItemInfo"])
		{
			ItemIconInfoView = new InfoView(new PictureBox() { SizeMode = PictureBoxSizeMode.CenterImage }, InfoView.TipDock.Top);
			ItemIconInfoView.Text = MainForm.CurrentLanguage["Icon"];
			ItemIconInfoView.Bounds = new Rectangle(5, 5, 80, 80);
			ItemIconInfoView.Tip.BackColor = ItemsColor;

			ItemNameInfoView = new InfoView(new TextBox() { TextAlign = HorizontalAlignment.Right }, InfoView.TipDock.Left, false);
			ItemNameInfoView.Text = MainForm.CurrentLanguage["Name"];
			ItemNameInfoView.Tip.BackColor = ItemsColor;
			ItemNameInfoView.Bounds = new Rectangle(0, 0, 170, 20);

			ItemTypeInfoView = new InfoView(new TextBox() { TextAlign = HorizontalAlignment.Right }, InfoView.TipDock.Left, false);
			ItemTypeInfoView.Text = MainForm.CurrentLanguage["Type"];
			ItemTypeInfoView.Tip.BackColor = ItemsColor;
			ItemTypeInfoView.Bounds = new Rectangle(0, 20, 170, 20);

			ItemRareInfoView = new InfoView(new TextBox() { TextAlign = HorizontalAlignment.Right }, InfoView.TipDock.Left, false);
			ItemRareInfoView.Text = MainForm.CurrentLanguage["Rare"];
			ItemRareInfoView.Tip.BackColor = ItemsColor;
			ItemRareInfoView.Bounds = new Rectangle(0, 40, 170, 20);

			InfoView ItemDetailInfoView = new InfoView(new Panel() { BorderStyle = BorderStyle.None }, InfoView.TipDock.Top);
			Panel ItemDetailInfoViewContent = (ItemDetailInfoView.View as Panel);
			ItemDetailInfoViewContent.Controls.Add(ItemNameInfoView);
			ItemDetailInfoViewContent.Controls.Add(ItemTypeInfoView);
			ItemDetailInfoViewContent.Controls.Add(ItemRareInfoView);
			ItemDetailInfoView.Text = MainForm.CurrentLanguage["Details"];
			ItemDetailInfoView.Tip.BackColor = ItemsColor;
			ItemDetailInfoView.Bounds = new Rectangle(90, 5, 170, 80);

			ItemDescriptionInfoView = new InfoView(new TextBox() { Multiline = true }, InfoView.TipDock.Left);
			ItemDescriptionInfoView.Text = MainForm.CurrentLanguage["Description"];
			ItemDescriptionInfoView.Tip.BackColor = ItemsColor;
			ItemDescriptionInfoView.Bounds = new Rectangle(5, 90, 255, 80);

			RequireItems = new MTabControl();
			ItemRecipeFromInfoView = new InfoView(RequireItems, InfoView.TipDock.Top);
			ItemRecipeFromInfoView.Text = MainForm.CurrentLanguage["Recipe"] + "(From)";
			ItemRecipeFromInfoView.Tip.BackColor = ItemsColor;
			ItemRecipeFromInfoView.Bounds = new Rectangle(5, 175, 255, 100);

			RecipeToItems = new ListBox()
			{
				BorderStyle = BorderStyle.None
			};
			RecipeToItems.MouseDoubleClick += (s, e) =>
			{
				OnRecipeToItemDoubleClick(s, e);
			};
			ItemRecipeToInfoView = new InfoView(RecipeToItems, InfoView.TipDock.Top);
			ItemRecipeToInfoView.Text = MainForm.CurrentLanguage["Recipe"] + "(To)";
			ItemRecipeToInfoView.Tip.BackColor = ItemsColor;
			ItemRecipeToInfoView.Bounds = new Rectangle(5, 280, 255, 100);

			ItemValueInfoView = new InfoView(new TextBox() { TextAlign = HorizontalAlignment.Center }, InfoView.TipDock.Left);
			ItemValueInfoView.Text = MainForm.CurrentLanguage["Rare"];
			ItemValueInfoView.Tip.BackColor = ItemsColor;
			ItemValueInfoView.Bounds = new Rectangle(5, 385, 255, 20);



			Controls.Add(ItemIconInfoView);
			Controls.Add(ItemDetailInfoView);
			Controls.Add(ItemDescriptionInfoView);
			Controls.Add(ItemRecipeFromInfoView);
			Controls.Add(ItemRecipeToInfoView);
			Controls.Add(ItemValueInfoView);
		}

		public void ResetData()
		{
			(ItemIconInfoView.View as PictureBox).Image = null;
			(ItemNameInfoView.View as TextBox).Text = "";
			(ItemTypeInfoView.View as TextBox).Text = "";
			(ItemRareInfoView.View as TextBox).Text = "";
			string desc = "";
			(ItemDescriptionInfoView.View as TextBox).Text = desc;
			(ItemRecipeFromInfoView.View as TabControl).TabPages.Clear();
			(ItemRecipeToInfoView.View as ListBox).Items.Clear();

			(ItemValueInfoView.View as TextBox).Text = "";

		}

		public void SetData(int index)
		{
			Image img = GameResLoader.ItemImages.Images[index.ToString()];
			(ItemIconInfoView.View as PictureBox).Image = img;
			(ItemNameInfoView.View as TextBox).Text = ItemsTabPage.Items[index]["Name"].ToString();
			(ItemTypeInfoView.View as TextBox).Text = ItemsTabPage.Items[index]["type"].ToString();
			(ItemRareInfoView.View as TextBox).Text = ItemsTabPage.Items[index]["rare"].ToString();
			//string desc = string.Concat(ItemsTabPage.Items[index]["ToolTip"]["_tooltipLines"].ToList().Select(t => t.ToString() + "\n"));
			//(ItemDescriptionInfoView.View as TextBox).Text = desc;
			(ItemRecipeFromInfoView.View as TabControl).TabPages.Clear();
			var pRe = ItemsTabPage.Recipes.Where(t => t["item"]["type"].ToObject<int>() == ItemsTabPage.Items[index]["type"].ToObject<int>());
			if (pRe.Count() > 0)
			{
				int t = 0;
				foreach (var recipe in pRe)
				{
					t++;
					var ritems = (recipe["rItems"] as JArray);
					TabPage page = new TabPage(t.ToString());
					RequireItems.TabPages.Add(page);
					ListBox box = new ListBox()
					{
						BorderStyle = BorderStyle.None
					};
					box.MouseDoubleClick += (s, e) =>
					{
						OnRequireItemDoubleClick(s, e);
					};
					box.Dock = DockStyle.Fill;
					page.Controls.Add(box);
					foreach (var itm in ritems)
					{
						var itemType = itm["type"].ToObject<int>();
						if (itemType != 0)
							box.Items.Add("[" + itemType + "] " + ItemsTabPage.Items[itemType]["Name"].ToString() + " [" + itm["stack"].ToObject<int>() + "]");
					}
				}
			}
			(ItemRecipeToInfoView.View as ListBox).Items.Clear();
			pRe = ItemsTabPage.Recipes.Where(
				t => (t["rItems"] as JArray).Where(
					y => index != 0 && y["type"].ToObject<int>() == index).Count() > 0);
			foreach (var p in pRe)
			{
				var itm = p["item"];
				(ItemRecipeToInfoView.View as ListBox).Items.Add("[" + itm["type"] + "] " + ItemsTabPage.Items[Convert.ToInt32(itm["type"])]["Name"].ToString() + " [" + itm["stack"] + "]");
			}

			(ItemValueInfoView.View as TextBox).Text = ItemsTabPage.GetValueString(Convert.ToInt32(ItemsTabPage.Items[index]["value"].ToString()));

		}
	}
}
