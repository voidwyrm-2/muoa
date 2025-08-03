local function createBuilder()
    local count = 0
    local cmds = {}
    local formatter = function(cmd)
        return "Running '" .. cmd .. "'"
    end

    return {
        add = function(cmd)
            count = count + 1
            cmds[count] = { cmd = cmd, formatter = formatter }
        end,

        setFormatter = function(newFormatter)
            formatter = newFormatter
        end,

        run = function()
            for i = 1, count do
                local cmd = cmds[i]

                print(cmd.formatter(cmd.cmd))

                local _, _, code = os.execute(cmd.cmd)

                if code ~= 0 then
                    print("Command '" .. cmd.cmd .. "' failed with exit code " .. tostring(code))
                    return
                end
            end
        end
    }
end

local builder = createBuilder()

builder.add("dotnet build")

builder.add("rm -rf pkg")
builder.add("mkdir pkg")

local targets = {
    osx = { "arm64", "x64" },
    linux = { "arm64", "x64" },
    win = { "arm64", "x64", "x86" },
}

for ost, arches in pairs(targets) do
    for _, arch in pairs(arches) do
        local pair = ost .. "-" .. arch
        local bin = "bin/%s/" .. pair

        builder.setFormatter(function(cmd)
            return "Running '" .. cmd .. "' for " .. pair
        end)

        local cmdBuild = string.format("dotnet build --os %s --arch %s -o %s", ost, arch, bin)
        builder.add(cmdBuild)

        local cmdMove = string.format("mv %s .", bin)
        builder.add(cmdMove)

        local cmdZip = string.format("zip -r pkg/%s %s", pair, pair)
        builder.add(cmdZip)

        builder.add("rm -rf " .. pair)
    end
end

builder.run()
