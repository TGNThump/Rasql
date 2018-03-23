[query] => [select] + ' ' + [from] + ' ' + [where]
[query] => [select] + ' ' + [from]

[select] => 'SELECT ' + [selectList]
[from] => 'FROM ' + [fromList]
[where] => 'WHERE ' + [condition]

[selectList] => '*'
[selectList] => [selectList] + [comma] + [selectElement]
[selectList] => [selectElement]

[selectElement] => [field]
[selectElement] => [field] + ' AS ' + [string]

[fromList] => [fromList] + [comma] + [fromElement]
[fromList] => [fromElement]
[join] => [fromElement] + ' JOIN ' + [fromElement] + ' ON ' + [condition]

[fromElement] => [join]
[fromElement] => [table]
[fromElement] => [fromElement] + ' AS ' + [table]
[fromElement] => '(' + [query] + ')' + [table]

[condition] => '(' + [condition] + ')'
[condition] => [value] + [operator] + [value]
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
[string] => [string] + [char]

[table] => [string]
[field] => [string]
[field] => [string] + '.' + [string]