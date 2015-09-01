# EmberJS.WebAPI
Allows working with the default RestAdapter in the Ember-Data library simply by adding this nuget package to an ASP.NET Web API project.


#Usage
`PM> Install-Package EmberJS.WebAPI`

#Example

With a c# class defined as:
```c#
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

Would normally be returned from webapi as this:


```json
[
    {
      "id": 10,
      "firstName": "John",
      "lastName": "Doe"
    },
    {
      "id": 11,
      "firstName": "Jane",
      "lastName": "Doe"
    }
]
```

This package will transform the json to what Ember-Data expects by wrapping it with `customers`:

```json
{
  "customers": [
    {
      "id": 10,
      "firstName": "John",
      "lastName": "Doe"
    },
    {
      "id": 11,
      "firstName": "Jane",
      "lastName": "Doe"
    }
  ]
}
```

Notice 2 things - First that the Customers is pluralized because a collection was returned and it uses the class name as the wrapper. Second that the variables are camel cased. 

If you want to change the default behavior and specify the name of the collection you can like so:

```c#
[JsonObject(Title="client")]
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```