using System;
using Gtk;
using GtkSharp;
using WebKit;

namespace monobrowser {
    class coolbrowser 
	{
		public static void Main (string[] args)
		{
			string url = (args.Length > 0) ? args[0] : "";

			Application.Init ();
			MainWindow window = new MainWindow (url);
			window.Show ();
			Application.Run ();
		}
	}

	public class MainWindow: Gtk.Window
	{
		const string APP_NAME = "CoolMonoBrowser";

		private string url = "http://firebirdsql.org";
		
		private Gtk.VBox vbox = null;
		private Gtk.MenuBar menubar = null;
		private Gtk.Toolbar toolbar = null;
		private Gtk.Toolbar findbar = null;
		private Gtk.Entry uri_entry = null;
		private Gtk.Entry find_entry = null;
		private WebKit.WebView webview = null;
		private Gtk.Statusbar statusbar = null;
		
		private Gtk.Action action_back;
		private Gtk.Action action_forward;
		private Gtk.Action action_reload;
		private Gtk.Action action_stop;
		private Gtk.Action action_jump;

		public MainWindow (string url): base (Gtk.WindowType.Toplevel)
		{
			if (url != "")
				this.url = url;

			CreateWidgets ();
			webview.Open (this.url);
		}
		
		private void CreateWidgets ()
		{
			this.Title = APP_NAME;
			this.SetDefaultSize (800, 600);
			this.DeleteEvent += new DeleteEventHandler (OnDeleteEvent);

			CreateActions ();
			CreateMenubar ();
			CreateToolbar ();
			CreateWebView ();
			CreateFindbar ();
			CreateStatusBar ();
					
			Gtk.ScrolledWindow scroll = new Gtk.ScrolledWindow ();
			scroll.Add (webview);

			vbox = new Gtk.VBox (false, 1);
			vbox.PackStart (menubar, false, false, 0);
			vbox.PackStart (toolbar, false, false, 0);
			vbox.PackStart (scroll);
			vbox.PackStart (findbar, false, false, 0);
			vbox.PackEnd (statusbar, false, true, 0);

			this.Add (vbox);
			this.ShowAll ();
		}
		
		private void CreateActions ()
		{
			action_back    = new Gtk.Action("go-back",    "Go Back",    null, "gtk-go-back");
			action_forward = new Gtk.Action("go-forward", "Go Forward", null, "gtk-go-forward");
			action_reload  = new Gtk.Action("reload",     "Reload",     null, "gtk-refresh");
			action_stop    = new Gtk.Action("stop",       "Stop",       null, "gtk-stop");
			action_jump    = new Gtk.Action("jump",       "Jump",       null, "gtk-jump-to");

			action_back.Activated    += new EventHandler(on_back_activate);
			action_forward.Activated += new EventHandler(on_forward_activate);
			action_reload.Activated  += new EventHandler(on_reload_activate);
			action_stop.Activated    += new EventHandler(on_stop_activate);
			action_jump.Activated    += new EventHandler(on_uri_activate);
		}
		
		private void CreateMenubar ()
		{
		  menubar = new MenuBar();
   
	      // Now define an accelerator group.
	      AccelGroup aGroup = new AccelGroup();
	      this.AddAccelGroup(aGroup);
	   
	      // Build File menu item.
	      Menu fileMenu = new Menu();
		  Menu editMenu = new Menu();
		  Menu viewMenu = new Menu();
	      MenuItem menuItem = new MenuItem("_File");
		  MenuItem editItem = new MenuItem("_Edit");
		  MenuItem viewItem = new MenuItem("_View");	
	      menuItem.Submenu = fileMenu;
		  editItem.Submenu = editMenu;
		  viewItem.Submenu = viewMenu;
		  
	      menubar.Append(menuItem);
		  menubar.Append(editItem);
		  menubar.Append(viewItem);
	   
		 
	
	      // File | Exit menu Item.
	      menuItem = new ImageMenuItem(Stock.Quit, aGroup);
		  
	      menuItem.Activated += new EventHandler(FileQuit_Activated);
		  fileMenu.Append(menuItem);
		  // Build Edit -> Undo Redo | Cut Copy Paste Select All menu Item.
		  MenuItem undoItem = new ImageMenuItem(Stock.Undo, aGroup);
		  editMenu.Append(undoItem);
		  MenuItem redoItem = new ImageMenuItem(Stock.Redo, aGroup);
		  editMenu.Append(redoItem);
			
		  MenuItem cutItem = new ImageMenuItem(Stock.Cut, aGroup);
		  editMenu.Append(cutItem);
		  MenuItem copyItem = new ImageMenuItem(Stock.Copy, aGroup);
		  editMenu.Append(copyItem);
		  MenuItem pasteItem = new ImageMenuItem(Stock.Paste, aGroup);
		  editMenu.Append(pasteItem);
		  MenuItem selectAllItem = new ImageMenuItem(Stock.SelectAll, aGroup);
		  editMenu.Append(selectAllItem);
		  MenuItem findItem = new ImageMenuItem(Stock.Find, aGroup);
		  editMenu.Append(findItem);
		  
			
			
		  
	      
				
			
		}
		
		private void CreateToolbar ()
		{
			// UrlEntry
			uri_entry = new Gtk.Entry ();
			uri_entry.Activated += new EventHandler(on_uri_activate);

			Gtk.ToolItem uri_item = new Gtk.ToolItem ();
			uri_item.Expand = true;
			uri_item.Add (uri_entry);

			// Toolbar
			toolbar = new Toolbar ();
			toolbar.ToolbarStyle = ToolbarStyle.Icons;
			toolbar.Orientation = Orientation.Horizontal;
			toolbar.ShowArrow = true;

			// Toolbar Items
			toolbar.Add (action_back.CreateToolItem());
			toolbar.Add (action_forward.CreateToolItem());
			toolbar.Add (action_reload.CreateToolItem());
			toolbar.Add (action_stop.CreateToolItem());
			toolbar.Add (uri_item);
			toolbar.Add (action_jump.CreateToolItem());
		}

		private void CreateWebView ()
		{
			webview = new WebView ();
			webview.Editable = false;
			
			//WebInspector inspector = webview.Inspector;
			
			webview.TitleChanged += new TitleChangedHandler (OnTitleChanged);
			webview.HoveringOverLink += new HoveringOverLinkHandler (OnHoveringOverLink);
			webview.LoadCommitted += new LoadCommittedHandler (OnLoadCommitted);
			webview.LoadFinished += new LoadFinishedHandler (OnLoadFinished);
			webview.LoadStarted += new LoadStartedHandler (OnLoadStarted);
			
			
		}

		private void CreateStatusBar ()
		{
			statusbar = new Gtk.Statusbar ();
		}

		private void CreateFindbar ()
		{
			// FindEntry
			find_entry = new Gtk.Entry ();
			//find_entry.Activated += new EventHandler(on_uri_activate);

			Gtk.ToolItem find_item = new Gtk.ToolItem ();
			//find_item.Expand = false;
			find_item.Add (find_entry);

			// Toolbar
			findbar = new Toolbar ();
			findbar.ToolbarStyle = ToolbarStyle.Icons;
			findbar.Orientation = Orientation.Horizontal;
			findbar.ShowArrow = true;

			// Toolbar Items
			findbar.Add (action_stop.CreateToolItem());
			findbar.Add (find_item);
			findbar.Add (action_back.CreateToolItem());
			findbar.Add (action_forward.CreateToolItem());
			find_entry.Hide();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}
		
		private void OnTitleChanged (object o, TitleChangedArgs args)
		{
			if (args.Title == String.Empty)
				this.Title = APP_NAME;
			else
				this.Title = String.Format ("{0} - {1}", args.Title, APP_NAME);
		}

		private void OnHoveringOverLink (object o, HoveringOverLinkArgs args)
		{
			statusbar.Pop (1);
			if (args.Link != null) {
				statusbar.Push (1, args.Link);
			}
		}

		private void OnLoadCommitted (object o, LoadCommittedArgs args)
		{
			action_back.Sensitive = webview.CanGoBack ();
			action_forward.Sensitive = webview.CanGoForward ();
			
			uri_entry.Text = args.Frame.Uri;
		}
		

		private void OnLoadFinished (object o, LoadFinishedArgs args)
		{
			action_stop.Sensitive = false;
		}
		
		private void OnLoadStarted (object o, LoadStartedArgs args)
		{
			action_stop.Sensitive = true;
		}


		private void on_back_activate (object o, EventArgs args)
		{
			webview.GoBack ();
		}

		private void on_forward_activate (object o, EventArgs args)
		{
			webview.GoForward ();
		}

		private void on_reload_activate (object o, EventArgs args)
		{
			webview.Reload ();
		}

		private void on_stop_activate (object o, EventArgs args)
		{
			webview.StopLoading ();
		}

		private void on_uri_activate (object o, EventArgs args)
		{
			webview.Open (uri_entry.Text);
		}
		
		void FileQuit_Activated(object sender, EventArgs args)
  		{         
    		Application.Quit();
  		}
	}
}
