folder = uigetdir;
files = dir(fullfile(folder,"*.csv"));
table = '';
for i = 3 : length(files)
    temp = readtable(fullfile(files(i).folder,files(i).name));
    for j = 1:height(temp)
        if(mod(j,2) == 0)
            temp{j,3} = 1;
        end
    end
    if height(table) == 0
        table = temp;
    else
        table = [table;temp];
    end
end
sortrows(table,"DisplayTime");