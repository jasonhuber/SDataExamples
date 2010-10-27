			var etag ="", key="";		
			//define the service.
			var service = new Sage.SData.Client.SDataService({
			serverName: 'localhost',
			virtualDirectory: 'sdata',
			applicationName: 'slx',
			contractName: 'dynamic',
			port: 3333,
			protocol: /https/i.test(window.location.protocol) ? 'https' : false,
			userName: 'lee',
			password: ''
			});
			
			//define the request, passing in the service object.
			var request = new Sage.SData.Client.SDataSingleResourceRequest(service)
			.setResourceKind('accounts');
		
		
			function AddAccount()
			{
		
				var that = this; //used for the callback.
				var entry = {
					$name: 'Account',
					AccountName: dojo.byId("tab1txtAccountName").value,
					Division: dojo.byId("tab1txtDivision").value,
					BusinessDescription: dojo.byId("tab1txtBusDesc").value
				};
				request.create(entry, {
					async: true,
					success: function(data) {
						
						SearchAccountbyID(data.$key);
					}
				});				
				
			}
			
			function SearchAccountbyID(key)
			{
				request.setResourceSelector("'" + key + "'");
				var that = this;
				request.read({
					async: true,
					success: function(data) {
						//my entire account that was just added comes back in data.
						dojo.byId("tab3txtAccountName").value = data.AccountName;
						dojo.byId("tab3txtDivision").value = data.Division;
						dojo.byId("tab3txtBusDesc").value = data.BusinessDescription;
						
						that.etag= data.$etag;
						that.key = data.$key;
						//after I insert the account I want to select it.
						
						dojo.byId("lblAccountID").innerHTML = "We are working with Account:" + that.key;
						
						//get a hold of the tab container so I can tell it to set the pane I want.
						var tabs = dijit.byId("tabContainer");
						var pane = dijit.byId("Tab3"); //get the tab I want to set.
						tabs.selectChild(pane); //set it.
					}
				});				
			}
			
			function UpdateAccount()
			{
				
				var entry = {
					$name: 'Account',
					$etag: etag,
					AccountName: dojo.byId("tab3txtAccountName").value,
					Division: dojo.byId("tab3txtDivision").value,
					BusinessDescription: dojo.byId("tab3txtBusDesc").value
				};
				
				request.update(entry, {
					async: true,
					success: function(data) {
						dojo.byId("lblAccountID").innerHTML = "Account: " + data.$key + " Updated!";
						dojo.byId("lblAccountID").style.color = "#FF0000";
					}
				});	
			}
			
			function SearchAccounts()
			{
				var requestcoll = new Sage.SData.Client.SDataResourceCollectionRequest(service)
				.setResourceKind('accounts');
				
				
				var that = this;
				var where = new Array();
				var args = new Array()
				args["where"] = ""
				if (dojo.byId("tab2txtAccountName").value.length>0)
				{
					args["where"] += "AccountName like '%" + dojo.byId("tab2txtAccountName").value + "%' or ";
				}
				if (dojo.byId("tab2txtDivision").value.length>0)
				{
				
						args["where"] += "Division like '%" +  dojo.byId("tab2txtDivision").value + "%' or ";
				}
				if (dojo.byId("tab2txtBusDesc").value.length>0)
				{
						args["where"] += "BusinessDescription like '%" + dojo.byId("tab2txtBusDesc").value + "%' or ";
				}
				//need to trim that last OR off... (really should build a better query.
				if(args["where"].length ===0)
				{
					
					return false;
				}
				else
				{
					args["where"] = args["where"].substr(0, args["where"].length-4)
					
				}
				
				requestcoll.setQueryArgs(args,true
				);
	
				
				requestcoll.read({
					async: true,
					success: function(data) {
						var ehs	= "";	
						dojo.forEach(data.$resources,function(entry)
						{
						//<a href="#" onclick="SearchAccountbyID('entry.$key')>entry.$Account</a>"
							ehs += RML.a({content: "AccountName:" + entry.AccountName + ", Division: " + entry.Division + ", BusDesc: " + entry.BusinessDescription  , onclick: "SearchAccountbyID('" + entry.$key + "'); dojo.byId('AccountList').innerHTML = ''", href: "#"});
							ehs += RML.br();
						}//end function
						
						);//end for each
						dojo.byId("AccountList").innerHTML = ehs;
					}//end function success
				});//end .read	
			}