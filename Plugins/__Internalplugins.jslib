mergeInto(LibraryManager.library,{

  Hello: function (str) {
	sendMessage(Pointer_stringify(str));
  },
  
    GetUITreeInterface: function () {
	GetUITreeInterfaceJS();
  },
});