[query] => [select] + ' ' + [from] + (' ' + [join])? + (' ' + [where])?

[select] => 'SELECT ' + [selectList]
[from] => 'FROM ' + [fromList]
[join] => 'JOIN ' + [fromElement] + ' ON ' + [condition]
[where] => 'WHERE' + [condition]

[selectList] => '*'
[selectList] => [selectElement] + ([comma] + [selectElement])*

[selectElement] => [field]
[selectElement] => [field] + ' AS ' + [string]

[fromList] => [fromElement] + ([comma] + [fromElement])*

[fromElement] => [table]
[fromElement] => [table] + ' AS ' + [table]
[fromElement] => '(' + [query] + ')'

[condition] => [value] + [operator] + [value]
[condition] => [condition] + ' AND ' + [condition]
[condition] => [condition] + ' OR ' + [condition]
[condition] => 'NOT ' + [condition]

[operator] => '='|'<>'|'>'|'<'|'<='|'>='

[value] => [field]
[value] => [numeric]
[value] => ('\''|'"') + [string] + ('\''|'"')
[value] => 'NULL'

[numeric] => [0-9]
[numeric] => [0-9] + '.' + [0-9]

[comma] => ','|', '

[string] => [a-zA-Z]

[table] => [string]
[field] => [string]
[field] => [string] + '.' + [string]