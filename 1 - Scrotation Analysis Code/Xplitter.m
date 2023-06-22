sepScaling = true;

if sepScaling; angles = [2,4,8]; folName = "scaling"; col = 2;
else angles = [0, -15, 15, -30, 30]; folName = "eccentricity"; col = 3; end

myDir = uigetdir; %gets directory
listing = dir(myDir);
mkdir(myDir + " separated " + folName);
for folder = 3:length(listing)
    for angle = angles
        mkdir(fullfile(myDir + " separated " + folName, listing(folder).name + " " + angle));
    end
    sublisting = dir(fullfile(myDir, listing(folder).name, '*.csv'));
    for file = 1:length(sublisting)
        table = readtable(fullfile(myDir, listing(folder).name, sublisting(file).name));
        for angle = angles
            writetable(table(table{:, col} == angle, :), fullfile(myDir + " separated "  + folName, ...
                listing(folder).name + " " + angle, sublisting(file).name));
        end
    end
end