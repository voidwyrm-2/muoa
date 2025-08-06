local options = {}


local parts = {}

for part in string.gmatch(arg[0], "([^/]+)") do
    parts[#parts + 1] = part
end

local pathPrefix = table.concat(parts, "/", 1, #parts - 1) .. "/"

for i = 1, #arg do
    options[arg[i]] = ""
end

local tests = {
    "join"
}

for i = 1, #tests do
    local name = tests[i]

    if options[name] or options["all"] then
        local path = pathPrefix .. name .. ".muoa"

        print("testing '" .. path .. "'")

        local _, _, code = os.execute("muoa -f " .. path)
        if code ~= 0 then
            print("test '" .. name .. "' failed")
            break
        end

        print("test '" .. name .. "' passed")
    end
end
