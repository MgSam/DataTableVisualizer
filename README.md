# DataTable Visualizer
Are you tired of Visual Studio's built-in, modal, slow, unsearchable DataTable Visualizer? Me too! That's why I made my own!

This Visual Studio Extension provides a **non-modal**, **dockable**, and **searchable** DataTable visualizer for Visual Studio. It contains a very powerful text filter that takes either free text, or OData-like queries for fantastic searching! It also makes it really simple to copy and paste the data into Excel if you need to!

To access the visualizer, you must be debugging and stopped at a breakpoint. Click the down arrow while hovering over a DataTable variable and select "ShineTools.DataTableVisualizer". (I made the name different so as not to clash with the crappy built in visualizer)

This is my first extension, so feedback is welcome!

![Preview.png](DataTableVisualizerExtension\Resources\Preview.png)

## Instructions:

*Note: This information is all available from within the grid as a tooltip on the filter icon. In other words, don't worry, you don't have to memorize it to get started.*

Type into the box to apply a filter. Either a free-text query that will run a 'contains' match against every column or an OData-like query which can target specific columns.

The filter icon will be orange if a valid OData-like filter is applied, yellow if a contains filter is applied, or empty if no filter is currently applied.    
    
The blue "orb" next to the filter icon indicates the filter string has been changed but not yet applied. Apply a filter by hitting "Enter".    
    
OData-like queries support the following operators: `eq`, `ne`, `gt`, `ge`, `lt`, `le`, `and`, `or`, `not`.    
Strings and dates must be single quoted for OData queries. Numbers and booleans should not contain quotes.    
All column names and string matches are case-insensitive.
  
Two special operators are also provided: `ct`, `rx`. These are "contains" and "regex", respectively.     
`ct` will turn whatever column it is applied against into a string and run a contains match.    
`rx` will turn whatever column it is applied against into a string and run a regex match, utilizing standard .NET regular expressions.     

The following OData functions are also provided: `startswith()`, `endswith()`, `substringof()`, `year()`, `month()`, `day()`, `hour()`, `minute()`, `second()`.
   
#### Examples: 
```   
age gt 21
address rx '..00 Pennslyvania Avenue'
uniqueid ct '5def'
DOB lt '1990-01-08'
DOB ne '1990-01-05' or not (Name eq 'Fred')
```
