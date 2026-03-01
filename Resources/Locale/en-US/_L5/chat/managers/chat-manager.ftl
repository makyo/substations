# Sign language
chat-manager-entity-sign-wrap-message = { PROPER($entity) ->
*[false] the {$entityName} signs, [color={$color}][italic]{$message}[/italic][/color]
[true] {$entityName} signs, [color={$color}][italic]{$message}[/italic][/color]
    }

chat-manager-entity-unknown-sign-wrap-message = { PROPER($entity) ->
*[false] [color={$color}]the {$entityName} gestures something.[/color]
[true] [color={$color}]{$entityName} gestures something.[/color]
    }
chat-manager-entity-sign-no-free-hands = You need at least one free hand to sign!

chat-manager-entity-subtle-wrap-message = [italic][color={$color}]{ PROPER($entity) ->
*[false] the {$entityName} {$message}[/color][/italic]
[true] {$entityName} {$message}[/color][/italic]
    }
