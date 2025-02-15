//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Protocol/Item.proto
namespace PB
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ItemSell")]
  public partial class ItemSell : global::ProtoBuf.IExtensible
  {
    public ItemSell() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _count;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int count
    {
      get { return _count; }
      set { _count = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GemSelect")]
  public partial class GemSelect : global::ProtoBuf.IExtensible
  {
    public GemSelect() {}
    
    private string _gemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"gemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string gemId
    {
      get { return _gemId; }
      set { _gemId = value; }
    }
    private int _count;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int count
    {
      get { return _count; }
      set { _count = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ItemInfo")]
  public partial class ItemInfo : global::ProtoBuf.IExtensible
  {
    public ItemInfo() {}
    
    private long _id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _itemId;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _count;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int count
    {
      get { return _count; }
      set { _count = value; }
    }
    private int _status;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemInfoSync")]
  public partial class HSItemInfoSync : global::ProtoBuf.IExtensible
  {
    public HSItemInfoSync() {}
    
    private readonly global::System.Collections.Generic.List<ItemInfo> _itemInfos = new global::System.Collections.Generic.List<ItemInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"itemInfos", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ItemInfo> itemInfos
    {
      get { return _itemInfos; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemUse")]
  public partial class HSItemUse : global::ProtoBuf.IExtensible
  {
    public HSItemUse() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _itemCount = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"itemCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int itemCount
    {
      get { return _itemCount; }
      set { _itemCount = value; }
    }
    private int _targetID = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"targetID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int targetID
    {
      get { return _targetID; }
      set { _targetID = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemUseRet")]
  public partial class HSItemUseRet : global::ProtoBuf.IExtensible
  {
    public HSItemUseRet() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _useCountDaily;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"useCountDaily", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int useCountDaily
    {
      get { return _useCountDaily; }
      set { _useCountDaily = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBoxUseBatch")]
  public partial class HSItemBoxUseBatch : global::ProtoBuf.IExtensible
  {
    public HSItemBoxUseBatch() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _itemCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemCount
    {
      get { return _itemCount; }
      set { _itemCount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBoxUseBatchRet")]
  public partial class HSItemBoxUseBatchRet : global::ProtoBuf.IExtensible
  {
    public HSItemBoxUseBatchRet() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemSellBatch")]
  public partial class HSItemSellBatch : global::ProtoBuf.IExtensible
  {
    public HSItemSellBatch() {}
    
    private readonly global::System.Collections.Generic.List<ItemSell> _items = new global::System.Collections.Generic.List<ItemSell>();
    [global::ProtoBuf.ProtoMember(1, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<ItemSell> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemSellBatchRet")]
  public partial class HSItemSellBatchRet : global::ProtoBuf.IExtensible
  {
    public HSItemSellBatchRet() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBuy")]
  public partial class HSItemBuy : global::ProtoBuf.IExtensible
  {
    public HSItemBuy() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _itemCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemCount
    {
      get { return _itemCount; }
      set { _itemCount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBuyRet")]
  public partial class HSItemBuyRet : global::ProtoBuf.IExtensible
  {
    public HSItemBuyRet() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _itemCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemCount
    {
      get { return _itemCount; }
      set { _itemCount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBuyAndUse")]
  public partial class HSItemBuyAndUse : global::ProtoBuf.IExtensible
  {
    public HSItemBuyAndUse() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _itemCount;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"itemCount", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int itemCount
    {
      get { return _itemCount; }
      set { _itemCount = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemBuyAndUseRet")]
  public partial class HSItemBuyAndUseRet : global::ProtoBuf.IExtensible
  {
    public HSItemBuyAndUseRet() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private int _useCountDaily;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"useCountDaily", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int useCountDaily
    {
      get { return _useCountDaily; }
      set { _useCountDaily = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemCompose")]
  public partial class HSItemCompose : global::ProtoBuf.IExtensible
  {
    public HSItemCompose() {}
    
    private string _itemId;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"itemId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string itemId
    {
      get { return _itemId; }
      set { _itemId = value; }
    }
    private bool _composeAll;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"composeAll", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool composeAll
    {
      get { return _composeAll; }
      set { _composeAll = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSItemComposeRet")]
  public partial class HSItemComposeRet : global::ProtoBuf.IExtensible
  {
    public HSItemComposeRet() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSGemCompose")]
  public partial class HSGemCompose : global::ProtoBuf.IExtensible
  {
    public HSGemCompose() {}
    
    private readonly global::System.Collections.Generic.List<GemSelect> _gems = new global::System.Collections.Generic.List<GemSelect>();
    [global::ProtoBuf.ProtoMember(1, Name=@"gems", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<GemSelect> gems
    {
      get { return _gems; }
    }
  
    private bool _composeAll;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"composeAll", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool composeAll
    {
      get { return _composeAll; }
      set { _composeAll = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HSGemComposeRet")]
  public partial class HSGemComposeRet : global::ProtoBuf.IExtensible
  {
    public HSGemComposeRet() {}
    
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}