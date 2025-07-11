
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace cfg.config
{
public partial class TbCard
{
    private readonly System.Collections.Generic.Dictionary<string, config.Card> _dataMap;
    private readonly System.Collections.Generic.List<config.Card> _dataList;
    
    public TbCard(ByteBuf _buf)
    {
        _dataMap = new System.Collections.Generic.Dictionary<string, config.Card>();
        _dataList = new System.Collections.Generic.List<config.Card>();
        
        for(int n = _buf.ReadSize() ; n > 0 ; --n)
        {
            config.Card _v;
            _v = global::cfg.config.Card.DeserializeCard(_buf);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
    }

    public System.Collections.Generic.Dictionary<string, config.Card> DataMap => _dataMap;
    public System.Collections.Generic.List<config.Card> DataList => _dataList;

    public config.Card GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public config.Card Get(string key) => _dataMap[key];
    public config.Card this[string key] => _dataMap[key];

    public void ResolveRef(Tables tables)
    {
        foreach(var _v in _dataList)
        {
            _v.ResolveRef(tables);
        }
    }

}

}

