select av.Id, a.Name, av.Value
from AttributeValues av, Attributes a, ProductAttributes pa
where av.AttributeId = a.Id and av.Id = pa.AttributeValueId 
	and pa.ProductId = 1