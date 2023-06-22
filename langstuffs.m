%{
folder = uigetdir;
files = dir(fullfile(folder,"*.csv"));
for i = 1 : length(files)
    temp = readtable(fullfile(files(i).folder,files(i).name), 'Delimiter',';');
    if ~exist("table")
        table = temp;
    else
        table = [table;temp];
    end
end
sortrows(table,"CaptureTime");
%writetable(table,fullfile(folder,"allTogether.csv"));
%}

%{.
folder = uigetdir;
files = dir(fullfile(folder,"*.csv"));
for i = 1 : length(files)
    temp = readtable(fullfile(files(i).folder,files(i).name));
    for j = 1:height(temp)
        if(mod(temp{j,4},2) == 1)
            temp{j,3} = 1;
        end
    end
    if ~exist("table")
        table = temp;
    else
        table = [table;temp];
    end
end
sortrows(table,"DisplayTime");
%writetable(table,fullfile(folder,"allTogether.csv"));
%}

