[query] => [projection]
[query] => [selection]
[query] => [rename]
[query] => [join]
[query] => [setOperation]
[query] => [string]
[query] =>  '(' + [query] + ')'


[projection] => [pi] + ' ' + [attributeList] + ' ('+ [query] + ')'

[pi] => 'PI'
[pi] => 'PROJECT'
[pi] => 'π'

[attributeList] => [string]
[attributeList] => [string] + [comma] + [attributeList]


[selection] => [sigma] + ' ' + [condition] + ' (' + [query] + ')'

[sigma] => 'SIGMA'
[sigma] => 'σ'
[sigma] => 'SELECT'

[condition] => '(' + [condition] + ')'
[condition] => [value] + [operator] + [value]
[condition] => [condition] + ' AND ' + [condition]
[condition] => [condition] + ' OR ' + [condition]
[condition] => 'NOT ' + [condition]


[rename] => [attRename]
[rename] => [relRename]

[attRename] => [rho] + ' ' + [field] + '/' + [string] + ' (' + [query] + ')'
[relRename] => [rho] + ' ' + [string] + ' (' + [query] + ')'

[rho] => 'RHO'
[rho] => 'ρ'
[rho] => 'RENAME'

[join] => [joinSymbol] + ' ' + [condition] + ' (' + [query] + [comma] + [query] + ')'
[join] => [joinSymbol] + '(' + [query] + [comma] + [query] + ')'

[joinSymbol] => 'JOIN' 
[joinSymbol] => '⋈'


[setOperation] => [union]
[setOperation] => [intersection]
[setOperation] => [cartesian]
[setOperation] => [difference]


[union] => [unionSymbol] + '(' + [query] + [comma] + [query] + ')'

[unionSymbol] => 'UNION'
[unionSymbol] => '∪'
[unionSymbol] => 'U'

[intersection] => [intersectSymbol] + '(' + [query] + [comma] + [query] + ')'

[intersectSymbol] => 'INTERSECT'
[intersectSymbol] => '∩'
[intersectSymbol] => 'N'

[cartesian] => 'X(' + [query] + [comma] + [query] + ')'

[difference] => '-(' + [query] + [comma] + [query] + ')'

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