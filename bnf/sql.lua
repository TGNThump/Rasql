[query] => [select] + ' ' + [from] + ' ' + [where]
[query] => [select] + ' ' + [from]

[select] => 'SELECT ' + [selectList]
[from] => 'FROM ' + [fromList]
[where] => 'WHERE ' + [condition]

[selectList] => '*'
[selectList] => [selectElement] + [comma] + [selectList]
[selectList] => [selectElement]

[selectElement] => [field] + ' AS ' + [string]
[selectElement] => [field]

[fromList] => [fromElement] + [comma] + [fromList]
[fromList] => [fromElement]

[fromElement] => [join]
[fromElement] => [table]
[fromElement] => [fromElement] + ' AS ' + [table]
[fromElement] => '(' + [query] + ') ' + [table]

[join] => [fromElement] + ' JOIN ' + [fromElement] + ' ON ' + [condition]

[condition] => '(' + [condition] + ')'
[condition] => [value] + [operator] + [value]
[condition] => [value] + " " + [operator] + " " +  [value]
[condition] => [condition] + ' AND ' + [condition]
[condition] => [condition] + ' OR ' + [condition]
[condition] => 'NOT ' + [condition]

[operator] => '='
[operator] => '<>'
[operator] => '>'
[operator] => '<'
[operator] => '<='
[operator] => '>='

[value] => [field]
[value] => [numeric]
[value] => [quote] + [string] + [quote]
[value] => [null]

[null] => 'NULL'

[int] => [0-9]
[int] => [int] + [0-9]

[numeric] => [int]
[numeric] => [int] + '.' + [int]

[quote] => '\''
[quote] => '"'

[comma] => ','
[comma] => ', '

[char] => [a-zA-Z]
[char] => [int]

[string] => [char]
[string] => [char] + [string]

[table] => [string]
[field] => [string]
[field] => [string] + '.' + [string]