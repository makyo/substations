## Rev Head

# L5
roles-antag-rev-head-name = Head Revolutionary
roles-antag-rev-head-objective = Your objective is to take over the station by converting people to your cause and killing all Leadership staff on station.

# L5
head-rev-role-greeting =
    You are a Head Revolutionary.
    You are tasked with removing all of Leadership from station via conversion, death or imprisonment.
    The Syndicate has sponsored you with a flash that converts the crew to your side.
    Beware, this won't work on those with a mindshield or wearing eye protection.
    Viva la revolución!

head-rev-briefing =
    Use flashes to convert people to your cause.
    Get rid of or convert all heads to take over the station.

head-rev-break-mindshield = The Mindshield was destroyed!

## Rev

roles-antag-rev-name = Revolutionary
roles-antag-rev-objective = Your objective is to ensure the safety and follow the orders of the Head Revolutionaries as well as getting rid or converting of all Command staff on station.

rev-break-control = {$name} has remembered their true allegiance!

# L5
rev-role-greeting =
    You are a Revolutionary.
    You are tasked with taking over the station and protecting the Head Revolutionaries.
    Get rid of all of or convert the Leadership staff.
    Viva la revolución!

rev-briefing = Help your head revolutionaries get rid of every head to take over the station.

## General

rev-title = Revolutionaries
rev-description = Revolutionaries are among us.

rev-not-enough-ready-players = Not enough players readied up for the game. There were {$readyPlayersCount} players readied up out of {$minimumPlayers} needed. Can't start a Revolution.
rev-no-one-ready = No players readied up! Can't start a Revolution.
rev-no-heads = There were no Head Revolutionaries to be selected. Can't start a Revolution.

rev-won = The Head Revs survived and successfully seized control of the station.

rev-lost = Leadership survived and killed all of the Head Revs.

# L5
rev-stalemate = All of the Head Revs and Leadership staff died. It's a draw.

rev-reverse-stalemate = Both Leadership staff and Head Revs survived.

rev-headrev-count = {$initialCount ->
    [one] There was one Head Revolutionary:
    *[other] There were {$initialCount} Head Revolutionaries:
}

rev-headrev-name-user = [color=#5e9cff]{$name}[/color] ([color=gray]{$username}[/color]) converted {$count} {$count ->
    [one] person
    *[other] people
}

rev-headrev-name = [color=#5e9cff]{$name}[/color] converted {$count} {$count ->
    [one] person
    *[other] people
}

## Deconverted window

rev-deconverted-title = Deconverted!
rev-deconverted-text =
    As the last headrev has died, the revolution is over.

    You are no longer a revolutionary, so be nice.
rev-deconverted-confirm = Confirm
