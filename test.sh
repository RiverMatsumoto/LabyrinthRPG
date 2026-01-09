#!/usr/bin/zsh
cat > docs/uml/game/deps.puml <<'EOF'
@startuml
left to right direction
hide members
hide circle
skinparam classAttributeIconSize 0
skinparam linetype ortho

EOF

# include every fragment except the wrapper itself
find docs/uml/game -type f -name '*.puml' ! -name 'deps.puml' -print0 \
| sort -z \
| xargs -0 -I{} printf "!include %s\n" "{}" >> docs/uml/game/deps.puml

echo '@enduml' >> docs/uml/game/deps.puml

